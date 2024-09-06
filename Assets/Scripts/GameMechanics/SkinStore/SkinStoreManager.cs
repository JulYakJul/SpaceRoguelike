using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;
using System.Collections;

public class SkinStoreManager : MonoBehaviour
{
    public GameObject[] skinObjects;
    public int[] skinPrices;
    private int[] skinsBought;
    private int currentSkinIndex;
    [SerializeField] private TMP_Text scoreText;

    private void OnEnable() => YandexGame.GetDataEvent += LoadSaveData;
    private void OnDisable() => YandexGame.GetDataEvent -= LoadSaveData;

    private void Awake()
    {
        if (skinObjects == null || skinPrices == null || skinObjects.Length == 0 || skinPrices.Length != skinObjects.Length)
        {
            Debug.LogError("Skin objects array or skinPrices array is not properly initialized.");
            return;
        }

        skinsBought = new int[skinPrices.Length];

        if (skinsBought.Length > 0)
        {
            skinsBought[0] = 1;
        }

        for (int i = 0; i < skinObjects.Length; i++)
        {
            TextMeshProUGUI priceLabel = skinObjects[i].transform.Find("HideSkin/BuySkinButton/SkinPrice").GetComponent<TextMeshProUGUI>();

            if (priceLabel != null)
            {
                priceLabel.text = skinPrices[i].ToString();
            }
            else
            {
                Debug.LogError($"Price label not found for skin {i}");
            }

            Image skinImage = skinObjects[i].transform.Find("Skin").GetComponent<Image>();

            GameObject selectSkin = skinObjects[i].transform.Find("SelectSkin").gameObject;
            if (selectSkin != null)
            {
                selectSkin.SetActive(i == currentSkinIndex);
            }
            else
            {
                Debug.LogError($"SelectSkin not found for skin {i}");
            }

            if (scoreText != null)
            {
                scoreText.text = YandexGame.savesData.score.ToString();
            }
        }
    }

    private void ApplyCurrentSkin()
    {
        for (int i = 0; i < skinObjects.Length; i++)
        {
            GameObject selectSkin = skinObjects[i].transform.Find("SelectSkin").gameObject;
            if (selectSkin != null)
            {
                selectSkin.SetActive(i == currentSkinIndex);
            }
            else
            {
                Debug.LogError($"SelectSkin not found for skin {i}");
            }
        }

        if (currentSkinIndex >= 0 && currentSkinIndex < skinObjects.Length)
        {
            Debug.Log($"Применяем скин: {currentSkinIndex}");
        }
        else
        {
            Debug.LogError("Invalid skin index");
        }
    }

    public void SetCurrentSkin(int index)
    {
        if (index < 0 || index >= skinObjects.Length || skinsBought[index] <= 0)
        {
            Debug.LogError("Skin not bought or invalid index");
            return;
        }

        currentSkinIndex = index;
        YandexGame.savesData.currentSkinIndex = index;
        SaveData();
        ApplyCurrentSkin();
    }

    private void Start()
    {
        if (YandexGame.SDKEnabled)
        {
            LoadSaveData();
        }
    }

    private void LoadSaveData()
    {
        if (YandexGame.savesData == null)
        {
            YandexGame.savesData = new SavesYG();
        }

        if (YandexGame.savesData.boughtSkins == null || YandexGame.savesData.boughtSkins.Length != skinPrices.Length)
        {
            YandexGame.savesData.boughtSkins = new int[skinPrices.Length];
            YandexGame.savesData.boughtSkins[0] = 1;
        }

        if (skinsBought == null || skinsBought.Length != skinPrices.Length)
        {
            Debug.LogError("skinsBought is null or has an incorrect length");
            return;
        }

        if (YandexGame.savesData.boughtSkins.Length == skinsBought.Length)
        {
            Array.Copy(YandexGame.savesData.boughtSkins, skinsBought, skinsBought.Length);
        }
        else
        {
            Debug.LogWarning("The length of boughtSkins in savesData does not match skinsBought.");
        }

        currentSkinIndex = YandexGame.savesData.currentSkinIndex;
        ApplyCurrentSkin();
    }

    public bool IsSkinBought(int index)
    {
        return index >= 0 && index < skinsBought?.Length && skinsBought[index] > 0;
    }

    public bool IsSkinSet(int index)
    {
        return index >= 0 && index < skinObjects?.Length && currentSkinIndex == index;
    }

    public void BuySkin(int index)
    {
        if (index < 0 || index >= skinObjects.Length)
        {
            Debug.LogError("Invalid skin index");
            return;
        }

        TextMeshProUGUI priceLabel = skinObjects[index].transform.Find("HideSkin/BuySkinButton/SkinPrice").GetComponent<TextMeshProUGUI>();

        if (priceLabel == null){
            Debug.LogError("Text not found");
        }

        if (YandexGame.savesData.score >= skinPrices[index])
        {
            YandexGame.savesData.score -= skinPrices[index];
            skinsBought[index] = 1;

            YandexGame.savesData.currentSkinIndex = index;
            currentSkinIndex = index;

            for (int i = 0; i < skinsBought.Length; i++)
            {
                Debug.Log($"Index {i}: {skinsBought[i]}");
            }

            if (scoreText != null)
            {
                scoreText.text = YandexGame.savesData.score.ToString();
            }

            SaveData();
        }
        else
        {
            if (priceLabel != null)
            {
                StartCoroutine(BlinkPriceLabel(priceLabel));
            }

            Debug.LogError("Not enough coins to buy the skin");
        }
    }

    private IEnumerator BlinkPriceLabel(TextMeshProUGUI priceLabel)
    {
        Color originalColor = priceLabel.color;
        Color blinkColor = Color.red;
        int blinkCount = 3;

        for (int i = 0; i < blinkCount; i++)
        {
            priceLabel.color = blinkColor;
            yield return new WaitForSeconds(0.2f);
            priceLabel.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }


    private void SaveData()
    {
        if (YandexGame.savesData == null)
        {
            YandexGame.savesData = new SavesYG();
        }

        YandexGame.savesData.boughtSkins = skinsBought;
        YandexGame.savesData.currentSkinIndex = currentSkinIndex;
        YandexGame.SaveProgress();
    }
}
