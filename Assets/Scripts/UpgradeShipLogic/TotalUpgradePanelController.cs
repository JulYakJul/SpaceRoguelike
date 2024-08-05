using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TotalUpgradePanelController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private Button[] upgradeButtons;
    [SerializeField] private TMP_Text[] buttonTexts;

    private enum UpgradeType
    {
        Weapon,
        BulletType,
        Health
    }

    private void Start()
    {
        InitializeUpgradePanel();
    }

    public void InitializeUpgradePanel()
    {
        UpgradeType[] upgradeTypes = (UpgradeType[])System.Enum.GetValues(typeof(UpgradeType));

        ShuffleArray(upgradeTypes);

        for (int i = 0; i < upgradeButtons.Length; i++)
        {
            UpgradeType upgradeType = upgradeTypes[i];
            SetButtonAction(upgradeButtons[i], upgradeType);
            SetButtonText(buttonTexts[i], upgradeType);
        }
    }

    private void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            T temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    private void SetButtonAction(Button button, UpgradeType upgradeType)
    {
        button.onClick.RemoveAllListeners();

        switch (upgradeType)
        {
            case UpgradeType.Weapon:
                button.onClick.AddListener(() => OnUpgradeSelected(() => playerController.UpgradeWeapon()));
                break;
            case UpgradeType.BulletType:
                button.onClick.AddListener(() => OnUpgradeSelected(() => playerController.UpgradeBulletType()));
                break;
            case UpgradeType.Health:
                button.onClick.AddListener(() => OnUpgradeSelected(() => playerController.IncreasingHealthScale(30)));
                break;
        }
    }

    private void SetButtonText(TMP_Text buttonText, UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Weapon:
                buttonText.text = "Upgrade Weapon";
                break;
            case UpgradeType.BulletType:
                buttonText.text = "Upgrade Bullet Type";
                break;
            case UpgradeType.Health:
                buttonText.text = "Increase Health";
                break;
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
    }
}
