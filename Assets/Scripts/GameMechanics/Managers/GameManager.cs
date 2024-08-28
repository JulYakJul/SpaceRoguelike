using UnityEngine;
using TMPro;
using YG;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject upgradePanelController;
    public TMP_Text ScoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (YandexGame.SDKEnabled == true)
        {
            LoadSaveCloud();
        }
    }

    public void LoadSaveCloud()
    {
        ScoreText.text = YandexGame.savesData.score.ToString();
    }

    public void AddCoin()
    {
        YandexGame.savesData.score++;
        ScoreText.text = YandexGame.savesData.score.ToString();
        SaveData();
    }

    public void SaveData()
    {
        YandexGame.SaveProgress();
    }
}
