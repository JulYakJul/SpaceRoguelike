using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class TotalUpgradePanelController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private Button[] upgradeButtons;
    [SerializeField] private TMP_Text[] buttonTexts;

    [SerializeField] private Image imageButton1;
    [SerializeField] private Image imageButton2;
    [SerializeField] private Image imageButton3;

    [Header("Upgrades")]
    [SerializeField] private int upgradeHealthScale;
    [SerializeField] private int upgradeFireRate;
    [SerializeField] private int upgradeDetectionRadius;
    [SerializeField] private int upgradeStrengthScale;

    [Header("Upgrade Sprites")]
    [SerializeField] private Sprite weaponSprite;
    [SerializeField] private Sprite bulletTypeSprite;
    [SerializeField] private Sprite healthSprite;
    [SerializeField] private Sprite fireRateSprite;
    [SerializeField] private Sprite detectionRadiusSprite;
    [SerializeField] private Sprite strengthScaleSprite;

    [SerializeField] private GameObject menuButton;

    private int weaponUpgradeCount = 0;
    private const int MaxWeaponUpgrades = 2;

    private int detectionRadiusUpgradeCount = 0;
    private const int MaxDetectionRadiusUpgrades = 3;

    private enum UpgradeType
    {
        Weapon,
        BulletType,
        Health,
        FireRate,
        DetectionRadius,
        StrengthScale
    }

    private void Start()
    {
        gameObject.SetActive(false);
        InitializeUpgradePanel();
    }

    private void OnDisable()
    {
        menuButton.SetActive(true);
    }

    void OnEnable()
    {
        menuButton.SetActive(false);
    }

    public void InitializeUpgradePanel()
    {
        List<UpgradeType> availableUpgrades = new List<UpgradeType>((UpgradeType[])System.Enum.GetValues(typeof(UpgradeType)));

        if (weaponUpgradeCount >= MaxWeaponUpgrades)
        {
            availableUpgrades.Remove(UpgradeType.Weapon);
        }

        if (detectionRadiusUpgradeCount >= MaxDetectionRadiusUpgrades)
        {
            availableUpgrades.Remove(UpgradeType.DetectionRadius);
        }

        ShuffleList(availableUpgrades);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            if (i < availableUpgrades.Count)
            {
                UpgradeType upgradeType = availableUpgrades[i];
                SetButtonAction(upgradeButtons[i], upgradeType);
                SetButtonText(buttonTexts[i], upgradeType);
                SetImageSpriteForButton(i, upgradeType);
                upgradeButtons[i].gameObject.SetActive(true);
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void SetButtonAction(Button button, UpgradeType upgradeType)
    {
        button.onClick.RemoveAllListeners();

        switch (upgradeType)
        {
            case UpgradeType.Weapon:
                button.onClick.AddListener(() => OnUpgradeSelected(() => 
                {
                    playerController.UpgradeWeapon();
                    weaponUpgradeCount++;
                }));
                break;
            case UpgradeType.BulletType:
                button.onClick.AddListener(() => OnUpgradeSelected(() => playerController.UpgradeBulletType()));
                break;
            case UpgradeType.Health:
                button.onClick.AddListener(() => OnUpgradeSelected(() => playerController.IncreasingHealthScale(upgradeHealthScale)));
                break;
            case UpgradeType.FireRate:
                button.onClick.AddListener(() => OnUpgradeSelected(() => playerController.UpgradeFireRate(upgradeFireRate)));
                break;
            case UpgradeType.DetectionRadius:
                button.onClick.AddListener(() => OnUpgradeSelected(() => 
                {
                    playerController.UpgradeDetectionRadius(upgradeDetectionRadius);
                    detectionRadiusUpgradeCount++;
                }));
                break;
            case UpgradeType.StrengthScale:
                button.onClick.AddListener(() => OnUpgradeSelected(() => playerController.UpgradeStrengthScale(upgradeStrengthScale)));
                break;
        }
    }

    private void SetButtonText(TMP_Text buttonText, UpgradeType upgradeType)
    {
        var stringTable = LocalizationSettings.StringDatabase.GetTable("LanguageSettings");

        switch (upgradeType)
        {
            case UpgradeType.Weapon:
                buttonText.text = stringTable["upg_weapon"].GetLocalizedString();
                break;
            case UpgradeType.BulletType:
                buttonText.text = stringTable["upg_bullet_speed"].GetLocalizedString();
                break;
            case UpgradeType.Health:
                buttonText.text = stringTable["increase_health"].GetLocalizedString();
                break;
            case UpgradeType.FireRate:
                buttonText.text = stringTable["upg_fire_rate"].GetLocalizedString();
                break;
            case UpgradeType.DetectionRadius:
                buttonText.text = stringTable["upg_detection_radius"].GetLocalizedString();
                break;
            case UpgradeType.StrengthScale:
                buttonText.text = stringTable["upg_strength_scale"].GetLocalizedString();
                break;
        }
    }

    private void SetImageSpriteForButton(int index, UpgradeType upgradeType)
    {
        Image targetImage = null;

        switch (index)
        {
            case 0:
                targetImage = imageButton1;
                break;
            case 1:
                targetImage = imageButton2;
                break;
            case 2:
                targetImage = imageButton3;
                break;
        }

        if (targetImage == null) return;

        Sprite spriteToSet = GetSpriteForUpgradeType(upgradeType);

        targetImage.sprite = spriteToSet;
    }

    private Sprite GetSpriteForUpgradeType(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Weapon:
                return weaponSprite;
            case UpgradeType.BulletType:
                return bulletTypeSprite;
            case UpgradeType.Health:
                return healthSprite;
            case UpgradeType.FireRate:
                return fireRateSprite;
            case UpgradeType.DetectionRadius:
                return detectionRadiusSprite;
            case UpgradeType.StrengthScale:
                return strengthScaleSprite;
            default:
                return null;
        }
    }

    private void OnUpgradeSelected(System.Action upgradeAction)
    {
        upgradeAction?.Invoke();
        if (playerController.IsUpgradePanelActive)
        {
            playerController.UpgradePanel.SetActive(false);
            Time.timeScale = 1f;
            playerController.IsUpgradePanelActive = false;
        }
        InitializeUpgradePanel();
    }
}
