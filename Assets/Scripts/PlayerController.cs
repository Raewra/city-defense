using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float jumpForce = 5f;
    public float maxSpeed = 6f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
    }

    void Update()
    {
        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // Movement input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Camera-relative movement
        Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        Vector3 move = (cameraRight * x + cameraForward * z).normalized;

        // Movement control (less in air)
        float control = isGrounded ? 1f : 0.5f;
        rb.AddForce(move * moveSpeed * control, ForceMode.Acceleration);

        // Clamp max horizontal speed
        Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (horizontalVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = horizontalVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }

        // Rotate player to face movement direction
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    // Ground detection using collisions
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
