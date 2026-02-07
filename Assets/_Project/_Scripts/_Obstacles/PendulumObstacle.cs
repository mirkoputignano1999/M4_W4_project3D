using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PendulumObstacle : MonoBehaviour
{
    [Header("Pendulum")]
    [SerializeField] private float amplitude = 1.3f;
    [SerializeField] private float frequency = 0.7f;
    [SerializeField] private Vector3 direction = Vector3.right;
    [SerializeField] private bool useLocalSpace = true;
    [SerializeField] private float phase = 0f;

    private Vector3 startWorldPos;
    private Vector3 startLocalPos;
    private Vector3 normDirection;

    private Rigidbody rb;
    private Transform parentTf;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        parentTf = transform.parent;

        startWorldPos = transform.position;
        startLocalPos = transform.localPosition;

        normDirection = direction.sqrMagnitude > 0.0001f
            ? direction.normalized
            : Vector3.right;
    }

    void FixedUpdate()
    {
        float displacement =
            amplitude * Mathf.Sin((2f * Mathf.PI * frequency * Time.time) + phase);

        Vector3 offset = normDirection * displacement;

        Vector3 targetWorldPos;

        if (useLocalSpace && parentTf != null)
        {
            // Movimento relativo al parent
            Vector3 localTarget = startLocalPos + offset;
            targetWorldPos = parentTf.TransformPoint(localTarget);
        }
        else
        {
            // Movimento in world space
            targetWorldPos = startWorldPos + offset;
        }

        rb.MovePosition(targetWorldPos);
    }

    void OnValidate()
    {
        amplitude = Mathf.Max(0f, amplitude);
        frequency = Mathf.Max(0f, frequency);
    }
}
