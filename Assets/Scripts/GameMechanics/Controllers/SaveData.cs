using UnityEngine;
using UnityEngine.UI;

public static class SaveData
{
    private const string ScoreKey = "PlayerScore";

    public static int Score
    {
        get => PlayerPrefs.GetInt(ScoreKey, 0);
        set => PlayerPrefs.SetInt(ScoreKey, value);
    }

    public static void AddScore(int value)
    {
        Score += value;
    }

    public static void ResetScore()
    {
        Score = 0;
    }
}
