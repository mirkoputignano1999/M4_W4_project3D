using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI timeText;

    void Update()
    {
        if (gameManager == null) return;

        coinsText.text = "Coins left: " + gameManager.CoinsLeft;

        float t = Mathf.Max(0f, gameManager.TimeLeft);
        timeText.text = "Time left: " + t.ToString("0");
    }
}
