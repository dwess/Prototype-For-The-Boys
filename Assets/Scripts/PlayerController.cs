using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Units per second")]
    public float moveSpeed = 5f;

    [Tooltip("Reference to CameraPivot (so we know screen‐up)")]
    public Transform cameraPivot;

    [Tooltip("Bullet prefab with Rigidbody attached")]
    public GameObject bulletPrefab;

    [Tooltip("Where bullets come from (child transform)")]
    public Transform firePoint;

    [Tooltip("Initial bullet speed")]
    public float bulletSpeed = 20f;

    [Tooltip("Degrees per second per unit of mouse X")]
    public float rotationSpeed = 120f;

    Rigidbody2D rb;
    Vector2 input;

    bool isMoving;
    Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isMoving = false;
    }

    void Update()
    {
        // 1) Gather movement input
        input.x = Input.GetAxisRaw("Horizontal"); // A/D, ←/→
        input.y = Input.GetAxisRaw("Vertical");   // W/S, ↑/↓
        float mx = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.forward, mx * rotationSpeed * Time.deltaTime, Space.World);


        // 2) Shooting
        if (Input.GetMouseButtonDown(0) && bulletPrefab != null && firePoint != null)
        {
            var b = Instantiate(bulletPrefab, firePoint.position + (transform.up * 1.2f) + (transform.right *.50f), transform.rotation);
            var rb3 = b.GetComponent<Rigidbody2D>();
            if (rb3 != null)
                rb3.linearVelocity = cameraPivot.up * bulletSpeed;
            Destroy(b, 5f);
        }
    }

    void FixedUpdate()
    {
        // Map input to world‐space relative to camera orientation:
        Vector3 right = transform.right;

        Vector3 up = transform.up; // since camera is looking down, forward==screen-up
        Vector3 move = (right * input.x + up * input.y).normalized;
        rb.linearVelocity = move * moveSpeed;
        isMoving = rb.linearVelocity.magnitude > 0;
        animator.SetBool("isMoving", isMoving);
        Debug.Log("" + isMoving);
    }
}
