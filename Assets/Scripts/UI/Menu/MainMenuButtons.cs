using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject skinStorePanel;
    public Animator skinStoreAnimator;

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void OpenSkinStore()
    {
        skinStoreAnimator.SetBool("isSkinStoreOpen", true);
    }

    public void MenuFromSkinStore()
    {
        skinStoreAnimator.SetBool("isSkinStoreOpen", false);
    }

    public void ExitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
