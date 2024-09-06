using UnityEngine;
using UnityEngine.UI;
using YG;

public class SkinButton : MonoBehaviour
{
    [SerializeField] private int skinIndex;
    [SerializeField] private GameObject purchaseButton;

    private SkinStoreManager skinStoreManager;
    private Button purchaseButtonComponent;

    private void Start()
    {
        skinStoreManager = FindObjectOfType<SkinStoreManager>();

        if (skinStoreManager != null)
        {
            Transform buyButtonTransform = purchaseButton.transform.Find("BuySkinButton");

            if (YandexGame.savesData.boughtSkins != null)
            {
                string boughtSkinsStr = string.Join(", ", YandexGame.savesData.boughtSkins);
            }
            else
            {
                Debug.Log("Купленные скины не инициализированы.");
            }

            if (buyButtonTransform != null)
            {
                purchaseButtonComponent = buyButtonTransform.GetComponent<Button>();
            }
            else
            {
                return;
            }

            if (skinStoreManager.IsSkinBought(skinIndex))
            {
                purchaseButton.SetActive(false);
            }
            else
            {
                purchaseButtonComponent.onClick.AddListener(OnPurchaseButtonClick);
            }

            GetComponent<Button>().onClick.AddListener(OnSkinSelect);
        }
        else
        {
            Debug.LogError("SkinStoreManager not found in the scene");
        }
    }

    private void OnSkinSelect()
    {
        if (skinStoreManager != null && skinStoreManager.IsSkinBought(skinIndex))
        {
            skinStoreManager.SetCurrentSkin(skinIndex);
        }
    }

    private void OnPurchaseButtonClick()
    {
        if (skinStoreManager != null && !skinStoreManager.IsSkinBought(skinIndex))
        {
            skinStoreManager.BuySkin(skinIndex);
            
            if (skinStoreManager.IsSkinBought(skinIndex))
            {
                purchaseButton.SetActive(false);
            }
        }
    }
}
