using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathHandler : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private string loseSceneName = "LoseScene";

    private bool isDead;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDeath.AddListener(OnDeath);
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDeath.RemoveListener(OnDeath);
    }

    private void OnDeath()
    {
        if (isDead) return;
        isDead = true;

        SceneManager.LoadScene(loseSceneName);
    }
}

