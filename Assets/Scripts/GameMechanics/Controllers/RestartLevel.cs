using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    public GameObject gameOverPanel; 

    private void Update()
    {
        if (gameOverPanel != null && gameOverPanel.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
