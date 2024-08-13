using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float startingTime = 30f;
    private float currentTime;
    [SerializeField] private TMP_Text timerText;
    public GameObject upgradePanelController;
    public GameObject totalUpgradePanelController;

    private bool wasUpgradePanelActive;

    void Start()
    {
        currentTime = startingTime;
        UpdateTimerText();
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = startingTime;
        }

        HandleUpgradePanelVisibility();
        UpdateTimerText();
    }

    void HandleUpgradePanelVisibility()
    {
        int seconds = Mathf.FloorToInt(currentTime % 60);

        if (seconds == 0)
        {
            if (upgradePanelController.activeSelf)
            {
                wasUpgradePanelActive = true;
                upgradePanelController.SetActive(false);
            }

            totalUpgradePanelController.SetActive(true);
        }

        if (seconds == 28)
        {
            if (wasUpgradePanelActive)
            {
                upgradePanelController.SetActive(true);
                wasUpgradePanelActive = false;
            }

            totalUpgradePanelController.SetActive(false);
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
