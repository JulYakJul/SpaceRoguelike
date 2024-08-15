using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;


public class UpgradePanelController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button optionButton1;
    [SerializeField] private Button optionButton2;
    [SerializeField] private Image optionImage1;
    [SerializeField] private Image optionImage2;
    [SerializeField] private TMP_Text optionText1;
    [SerializeField] private TMP_Text optionText2;

    [Header("Icons")]
    [SerializeField] private Sprite healthIcon;
    [SerializeField] private Sprite strengthIcon;
    [SerializeField] private Sprite speedIcon;

    private PlayerController player;

    private void Start()
    {
        upgradePanel.SetActive(false);
        player = FindObjectOfType<PlayerController>();

        optionButton1.onClick.AddListener(() => OnUpgradeOptionSelected(1));
        optionButton2.onClick.AddListener(() => OnUpgradeOptionSelected(2));
    }

    private void Update()
    {
        if (upgradePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                OnUpgradeOptionSelected(1);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                OnUpgradeOptionSelected(2);
            }
        }
    }

    public void ShowUpgradePanel(UpgradeType type1, int value1, UpgradeType type2, int value2)
    {
        SetupOption(optionImage1, optionText1, optionButton1, type1, value1);
        SetupOption(optionImage2, optionText2, optionButton2, type2, value2);
        upgradePanel.SetActive(true);
    }

    private void SetupOption(Image image, TMP_Text text, Button button, UpgradeType type, int value)
    {
        image.sprite = GetIconForUpgradeType(type);

        var stringTable = LocalizationSettings.StringDatabase.GetTable("LanguageSettings");

        string localizedUpgradeType = type switch
        {
            UpgradeType.Health => stringTable["health"].GetLocalizedString(),
            UpgradeType.Strength => stringTable["strength"].GetLocalizedString(),
            UpgradeType.Speed => stringTable["speed"].GetLocalizedString(),
            _ => type.ToString()
        };

        text.text = $"+{value} {localizedUpgradeType}";
        SetButtonBackgroundColor(button, type);
    }


    private Sprite GetIconForUpgradeType(UpgradeType type)
    {
        return type switch
        {
            UpgradeType.Health => healthIcon,
            UpgradeType.Strength => strengthIcon,
            UpgradeType.Speed => speedIcon,
            _ => null,
        };
    }

    private void SetButtonBackgroundColor(Button button, UpgradeType type)
    {
        Color backgroundColor = type switch
        {
            UpgradeType.Health => new Color(1f, 0f, 0f, 0.1f),
            UpgradeType.Strength => new Color(0f, 0f, 1f, 0.1f),
            UpgradeType.Speed => new Color(0f, 1f, 0f, 0.1f),
            _ => new Color(1f, 1f, 1f, 0.1f),
        };
        button.GetComponent<Image>().color = backgroundColor;
    }

    private void OnUpgradeOptionSelected(int option)
    {
        TMP_Text selectedText = option == 1 ? optionText1 : optionText2;
        ApplyUpgrade(selectedText.text);
        upgradePanel.SetActive(false);
    }

    private void ApplyUpgrade(string upgradeText)
    {
        string[] parts = upgradeText.Split(' ');
        int value = int.Parse(parts[0].Replace("+", ""));

        var stringTable = LocalizationSettings.StringDatabase.GetTable("LanguageSettings");

        if (parts.Length > 1)
        {
            if (parts[1] == stringTable["health"].GetLocalizedString())
            {
                player.IncreaseHealth(value);
            }
            else if (parts[1] == stringTable["strength"].GetLocalizedString())
            {
                player.UpgradeStrength(value);
            }
            else if (parts[1] == stringTable["speed"].GetLocalizedString())
            {
                player.UpgradeSpeed(value);
            }
        }
    }

}
