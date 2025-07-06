using UnityEngine;
using System.Collections.Generic;

public class EnemyController2 : Seeker
{
    [Tooltip("Units per second")]
    public float moveSpeedMin = 1f;
    [Tooltip("Units per second")]
    public float moveSpeedMax = 3f;

    private float moveSpeed;

    private Vector3 moveTarget;

    private Animator animator;

    private Rigidbody2D rb;

    private bool isMoving;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setNewRandomUnseenMoveTarget();

        rb = GetComponent<Rigidbody2D>();
        moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);

        animator = GetComponent<Animator>();
        isMoving = false;
        lastPosition = null;

        GeneratePath();
    }

    // Update is called once per frame
    // void  Update()
    // {

    //     //if (!CanSee(target, transform.position, LayerMask.GetMask("Obstacle")))
    //     //{
    //     //if (IsStuck())
    //     //    setNewRandomUnseenMoveTarget();
    //     //moveToTarget();
    //     if (hasPathMarkers)
    //         MoveAlongPath();
    //     //}
    //     //else
    //     //{
    //     //    isMoving = false;
    //     //    rb.linearVelocity = new Vector2(0f, 0f);
    //     //}

    //     animator?.SetBool("isMoving", isMoving);
    //     //lastPosition = transform;
    // }

    private void moveToTarget()
    {
        isMoving = true;
        rb.linearVelocity = moveSpeed * (moveTarget - transform.position).normalized;
        if (reachedPosition())
        {
            //setNewRandomUnseenMoveTarget();

            //moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
        }
    }

    private bool reachedPosition() => (moveTarget - transform.position).magnitude < 1f;

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
