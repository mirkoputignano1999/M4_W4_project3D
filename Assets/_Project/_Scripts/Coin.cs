using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum CoinType
    {
        Score,
        Time
    }

    [Header("Coin")]
    [SerializeField] private CoinType type = CoinType.Score;
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private float timeValue = 15f;

    private void OnTriggerEnter(Collider other)
    {
        //Controlla se il collider è del player
        if (!other.CompareTag("Player")) return;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            if (type == CoinType.Score)
                gm.AddScore(scoreValue);
            else
                gm.AddTime(timeValue);

            gm.CollectCoin();
        }

        Destroy(gameObject);
    }
}
