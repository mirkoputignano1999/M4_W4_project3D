using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private bool destroyOnHit = true;
    [Tooltip("Se true il collider deve essere 'isTrigger'")]
    [SerializeField] private bool useTrigger = true;

    private void Reset()
    {
        Collider c = GetComponent<Collider>();
        if (c != null && useTrigger) c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            if (destroyOnHit) Destroy(gameObject);
            return;
        }

        // se il player ha collider figlio, cerca anche nei parent
        var healthInParent = other.GetComponentInParent<Health>();
        if (healthInParent != null)
        {
            healthInParent.TakeDamage(damage);
            if (destroyOnHit) Destroy(gameObject);
        }
    }

    // alternativa per collider non trigger:
    private void OnCollisionEnter(Collision collision)
    {
        if (!useTrigger)
        {
            var health = collision.collider.GetComponent<Health>() ?? collision.collider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                if (destroyOnHit) Destroy(gameObject);
            }
        }
    }
}
