using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Position")]
    [SerializeField] private float distance = 3f;
    [SerializeField] private float height = 1.6f;
    [SerializeField] private float shoulderOffset = 0.4f;

    [Header("Collision")]
    [SerializeField] private float cameraRadius = 0.25f;
    [SerializeField] private LayerMask obstacleLayers;

    [Header("Smoothing / Rotation")]
    [SerializeField] private float smoothSpeed = 8f;
    [Tooltip("Max rotation degrees per second for camera look (prevents spin)")]
    [SerializeField] private float maxRotationDegPerSec = 360f;
    [Tooltip("Minimum allowed distance between camera and player to avoid instability")]
    [SerializeField] private float minDistanceFromTarget = 0.8f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Direzione stabile (basata sul player, proiettata su XZ)
        Vector3 backDir = -target.forward;
        backDir.y = 0f;
        if (backDir.sqrMagnitude < 0.0001f)
            backDir = -Vector3.forward;
        backDir.Normalize();

        // Posizione ideale (incluso shoulder offset)
        Vector3 idealPos = target.position + backDir * distance + Vector3.up * height + target.right * shoulderOffset;

        // SphereCast: partiamo da un punto alto-davanti al player verso la camera
        // in modo da evitare che il cast inizi all'interno del collider del player.
        Vector3 from = target.position + Vector3.up * (height * 0.9f) + target.forward * 0.2f;
        Vector3 dirVec = idealPos - from;
        float dist = dirVec.magnitude;
        Vector3 finalPos = idealPos;

        // Escludi layer del player dal mask per evitare che la camera colpisca il player stesso
        int playerLayer = target.gameObject.layer;
        int mask = obstacleLayers & ~(1 << playerLayer);

        if (dist > 0.0001f)
        {
            Vector3 dir = dirVec / dist;
            if (Physics.SphereCast(from, cameraRadius, dir, out RaycastHit hit, dist, mask, QueryTriggerInteraction.Ignore))
            {
                // Posiziona la camera appena prima dell'ostacolo lungo il raggio
                float safeDistance = Mathf.Max(0f, hit.distance - cameraRadius);
                finalPos = from + dir * safeDistance;

                // Se lo spazio è troppo piccolo, sposta la camera lungo la normale del punto d'impatto
                if ((finalPos - target.position).magnitude < minDistanceFromTarget)
                {
                    finalPos = hit.point + hit.normal * (cameraRadius + 0.05f);
                }
            }
        }

        // Assicura distanza minima dal target per evitare che finalPos coincida col target
        Vector3 offset = finalPos - target.position;
        if (offset.magnitude < minDistanceFromTarget)
        {
            if (offset.sqrMagnitude < 0.0001f)
                offset = -backDir * minDistanceFromTarget;
            finalPos = target.position + offset.normalized * minDistanceFromTarget;
        }

        // Movimento smooth della posizione
        transform.position = Vector3.Lerp(transform.position, finalPos, 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime));

        // ROTAZIONE STABILE: calcola lookDir e limita aggiornamento se troppo piccolo
        Vector3 lookTarget = target.position + Vector3.up * (height * 0.5f);
        Vector3 lookDir = lookTarget - transform.position;

        const float minLookSqr = 0.0005f;
        if (lookDir.sqrMagnitude > minLookSqr)
        {
            Quaternion desired = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            // Limita velocità di rotazione per evitare spin improvvisi
            float maxDelta = maxRotationDegPerSec * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, maxDelta);
        }
        // altrimenti non cambiare rotazione (evita calcoli instabili)
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}
