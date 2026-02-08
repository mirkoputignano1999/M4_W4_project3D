using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Coins")]
    [SerializeField] private int totalCoins;
    private int collectedCoins;

    [Header("Score")]
    private int score;

    [Header("Time")]
    [SerializeField] private float startTime = 60f;
    private float currentTime;

    [Header("Scenes")]
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string loseSceneName = "LoseScene";

    public int Score => score;
    public float TimeLeft => currentTime;
    public int CoinsLeft => totalCoins - collectedCoins;

    void Start()
    {
        currentTime = startTime;

        // Conta automaticamente le monete nella scena
        totalCoins = FindObjectsOfType<Coin>().Length;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            SceneManager.LoadScene(loseSceneName);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
    }

    public void AddTime(float amount)
    {
        currentTime += amount;
    }

    public void CollectCoin()
    {
        collectedCoins++;

        if (collectedCoins >= totalCoins)
        {
            SceneManager.LoadScene(winSceneName);
        }
    }
}

