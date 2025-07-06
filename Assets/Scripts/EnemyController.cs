using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [Tooltip("Units per second")]
    public float moveSpeedMin = 6f;
    [Tooltip("Units per second")]
    public float moveSpeedMax = 15f;

    private float moveSpeed;

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

    private Vector3 moveTarget;

    [Tooltip("The player")]
    public Transform targetTrans;
    private Animator animator;

    private Rigidbody2D rb;

    private bool isMoving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setNewRandomMoveTarget();

        rb = GetComponent<Rigidbody2D>();
        moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);

        animator = GetComponent<Animator>();
        isMoving = false;
        lastPosition = null;
    }

    // Update is called once per frame
    void Update()
    {

        if (CanSee(targetTrans, transform.position, LayerMask.GetMask("Default")))
        {
            if (IsStuck())
                setNewRandomMoveTarget();
            moveToTarget();
        }
        else
        {
            isMoving = false;
            rb.linearVelocity = new Vector2(0f, 0f);
        }

        animator.SetBool("isMoving", isMoving);
        lastPosition = transform;
    }

    private void moveToTarget()
    {
        isMoving = true;
        rb.linearVelocity = moveSpeed * (moveTarget - transform.position).normalized;
        if (reachedPosition())
        {
            setNewRandomMoveTarget();

            moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
        }
    }

    private bool CanSee(Transform enemy, Vector3 targetPosition, LayerMask obstacleMask)
    {
        Vector2 origin = enemy.position;
        Vector2 direction = (targetPosition - enemy.position).normalized;
        float distance = Vector2.Distance(enemy.position, targetPosition);

        // Raycast in 2D
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleMask);
        if (hit.collider != null)
        {
            // Debug.DrawRay(origin, direction * hit.distance, Color.red);
            //Debug.Log($"Sight blocked by: {hit.collider.name}");
            return false;
        }

        //Debug.DrawRay(origin, direction * distance, Color.green);
        //Debug.Log("nothing");
        return true;
    }


    private bool reachedPosition() => (moveTarget - transform.position).magnitude < 1f;

    private void setNewRandomMoveTarget()
    {
        moveTarget = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
        for (int i = 0; i < 100 && CanSee(targetTrans, moveTarget, LayerMask.GetMask("Default")); i++)
            moveTarget = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
    }


    private Transform lastPosition;
    private bool IsStuck() {
        //Enemies will get stuck when moving to a target, even when seen. Check last and current position
        if(transform.position == lastPosition?.position){
            Debug.Log("I'm stuck! Oh god oh god oh god");
            return true;
        }
        return false;
    }
}
