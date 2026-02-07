using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int startHealth = -1; // -1 = usa maxHealth
    [SerializeField] private float invincibilityTime = 0.5f;

    public UnityEvent<int> OnHealthChanged; // param: current health
    public UnityEvent OnDeath;
    public UnityEvent<int> OnDamaged; // param: damage amount

    private int currentHealth;
    private bool invincible;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;

    private void Start()
    {
        currentHealth = startHealth < 0 ? maxHealth : Mathf.Clamp(startHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        if (invincible) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnDamaged?.Invoke(amount);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        else
        {
            if (invincibilityTime > 0f)
                StartCoroutine(InvincibilityCoroutine());
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void Kill()
    {
        currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth);
        OnDeath?.Invoke();
    }

    private IEnumerator InvincibilityCoroutine()
    {
        invincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        invincible = false;
    }
}