using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image[] hearts; // ordina da sinistra a destra
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private void Start()
    {
        if (health == null) Debug.LogWarning("HeartsUI: Health non assegnata.");
        if (health != null)
        {
            health.OnHealthChanged.AddListener(UpdateHearts);
            // inizializza
            UpdateHearts(health.CurrentHealth);
        }
    }

    private void UpdateHearts(int current)
    {
        int max = Mathf.Max(health.MaxHealth, hearts.Length);
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < current)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
            hearts[i].enabled = i < health.MaxHealth; // disabilita eventuali immagini in eccesso
        }
    }
}
