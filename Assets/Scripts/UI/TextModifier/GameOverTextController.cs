using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class GameOverTextController : MonoBehaviour
{
    [SerializeField] private TMP_Text gameOverText; 

    private string[] localizationKeys = new string[]
    {
        "game_over_phrase_1",
        "game_over_phrase_2",
        "game_over_phrase_3",
        "game_over_phrase_4"
    };

    private void OnEnable()
    {
        SetRandomGameOverText();
    }

    private void SetRandomGameOverText()
    {
        string randomKey = localizationKeys[Random.Range(0, localizationKeys.Length)];

        LocalizedString localizedString = new LocalizedString("LanguageSettings", randomKey);
        localizedString.StringChanged += UpdateGameOverText;
    }

    private void UpdateGameOverText(string localizedText)
    {
        gameOverText.text = localizedText;
    }
}
