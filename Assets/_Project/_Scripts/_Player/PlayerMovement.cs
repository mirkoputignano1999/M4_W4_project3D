using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [Tooltip("Velocità orizzontale minima per permettere la rotazione (m/s)")]
    [SerializeField] private float rotationThreshold = 0.15f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Health / Fall")]
    [SerializeField] private Health health;
    [Tooltip("Y sotto la quale il player viene considerato caduto")]
    [SerializeField] private float fallDeathY = -10f;
    [SerializeField] private Transform respawnPoint;

    private Rigidbody rb;
    private Vector3 moveInput;

    private bool isGrounded;
    private int groundContacts;

    private AnimationParamHandler anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<AnimationParamHandler>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Disabilita Root Motion se l'Animator è presente
        var animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.applyRootMotion = false;

        if (health == null)
            health = GetComponent<Health>();
    }

    void Update()
    {
        // INPUT
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0f, v);
        if (input.sqrMagnitude > 1f) input.Normalize();

        // Movimento relativo alla camera
        Transform cam = Camera.main ? Camera.main.transform : null;
        if (cam != null)
        {
            Vector3 camForward = cam.forward;
            camForward.y = 0f;
            if (camForward.sqrMagnitude < 0.0001f) camForward = Vector3.forward;
            camForward.Normalize();

            Vector3 camRight = cam.right;
            camRight.y = 0f;
            if (camRight.sqrMagnitude < 0.0001f) camRight = Vector3.right;
            camRight.Normalize();

            moveInput = camForward * input.z + camRight * input.x;
        }
        else
        {
            moveInput = input;
        }

        // JUMP
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(
                rb.velocity.x,
                Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y),
                rb.velocity.z
            );
        }

        // FALL CHECK (solo in Update per reattività)
        if (transform.position.y < fallDeathY && health != null)
        {
            // assegna 1 danno per caduta, respawna
            health.TakeDamage(1);
            Respawn();
        }
    }

    void FixedUpdate()
    {
        // Movimento: manteniamo controllo diretto della velocità orizzontale
        Vector3 velocity = rb.velocity;
        velocity.x = moveInput.x * speed;
        velocity.z = moveInput.z * speed;
        rb.velocity = velocity;

        // Calcola velocità orizzontale reale
        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float horizontalSpeed = horizontalVel.magnitude;

        // Rotazione: esegui solo se la velocità orizzontale reale supera la soglia
        if (horizontalSpeed > rotationThreshold)
        {
            Vector3 lookDir = horizontalVel.normalized; // usa la direzione della velocità reale
            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);

            float maxDegThisFrame = rotationSpeed * 100f * Time.fixedDeltaTime;
            Quaternion newRot = Quaternion.RotateTowards(rb.rotation, targetRot, maxDegThisFrame);
            rb.MoveRotation(newRot);

            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

        // Aggiorna animator con la velocità reale
        if (anim != null)
        {
            anim.SetSpeed(horizontalSpeed / Mathf.Max(0.0001f, speed));
            anim.SetIsJumping(!isGrounded);
        }
    }

    private void Respawn()
    {
        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            transform.position = Vector3.up * 2f; // default

        rb.velocity = Vector3.zero;
    }

    // GROUND CHECK ROBUSTO
    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            groundContacts++;
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            groundContacts--;
            isGrounded = groundContacts > 0;
        }
    }
}
