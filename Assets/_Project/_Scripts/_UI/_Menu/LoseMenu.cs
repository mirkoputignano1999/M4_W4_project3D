using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    public void TryAgain()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}