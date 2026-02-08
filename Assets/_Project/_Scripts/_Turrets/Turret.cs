using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private float detectionRange = 6f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float projectileSpeed = 10f;

    [Header("Aiming")]
    [Tooltip("Soglia di distanza (m) sotto la quale il turret non cerca di ruotare per evitare jitter")]
    [SerializeField] private float minAimDistance = 0.5f;
    [Tooltip("Velocità di inseguimento della rotazione (gradi/sec)")]
    [SerializeField] private float aimSpeed = 360f;

    private float fireTimer;

    void Update()
    {
        if (target == null || firePoint == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= detectionRange)
        {
            AimAtTarget();
            Shoot();
        }
    }

    private void AimAtTarget()
    {
        // Direzione dal turret (pivot) al target, proiettata sull'asse Y=0 (solo orizzontale)
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < (minAimDistance * minAimDistance)) return;

        Quaternion lookRot = Quaternion.LookRotation(dir.normalized, Vector3.up);

        // Mantieni solo yaw (0, y, 0) e ruota in modo smussato per evitare scatti
        Quaternion targetRot = Quaternion.Euler(0f, lookRot.eulerAngles.y, 0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, aimSpeed * Time.deltaTime);
    }

    private void Shoot()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            fireTimer = 1f / Mathf.Max(0.0001f, fireRate);

            GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Calcola la direzione vera dal firePoint al target (includendo l'altezza).
                Vector3 shootDir = (target.position - firePoint.position);
                if (shootDir.sqrMagnitude < 0.0001f)
                    shootDir = firePoint.forward;
                else
                    shootDir.Normalize();

                // Imposta la rotazione del proiettile così che il suo forward punti verso il target
                proj.transform.rotation = Quaternion.LookRotation(shootDir, Vector3.up);

                // Assegna velocità basata sulla direzione calcolata (non sul forward del firePoint)
                rb.velocity = shootDir * projectileSpeed;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}