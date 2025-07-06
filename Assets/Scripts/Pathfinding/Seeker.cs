using UnityEngine;
using System.Collections.Generic;

public class Seeker : MonoBehaviour
{
    [SerializeField] protected PathfindingGrid grid;

    //Where THIS wants to go
    [SerializeField] protected Transform target;
    private float maxMoveSpeed = 5f;
    //used for avoiding
    [SerializeField] protected Transform enemy;

    protected List<Vector2> path = new List<Vector2>();
    protected int currentWaypoint = 0;
    protected bool hasPathMarkers = false;

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GeneratePath();
        }

        if (hasPathMarkers)
        {
            MoveAlongPath();
        }

        if (CanSee(target, transform.position, LayerMask.GetMask("Obstacle")))
        {
            setNewRandomUnseenMoveTarget();
            GeneratePath();
        }

        
    }

    protected virtual void GeneratePath()
    {
        path = grid.GetPath(transform, target.position);

        if (path != null && path.Count > 0)
        {
            currentWaypoint = 0;
            hasPathMarkers = true;
        }
    }

    protected virtual void MoveAlongPath()
    {
        if (currentWaypoint >= path.Count)
        {
            hasPathMarkers = false;
            return;
        }

        Vector2 targetPosition = path[currentWaypoint];
        Vector2 currentPosition = transform.position;

        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, maxMoveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.05f)
        {
            currentWaypoint++;
        }
    }

    protected virtual bool CanSee(Transform enemy, Vector3 targetPosition, LayerMask obstacleMask)
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

    public void setNewRandomUnseenMoveTarget()
    {
        Vector3 newMoveTarget = grid.getRandomAccessibleGridNode();
        if (enemy != null)
        {
            for (int i = 0; i < 100 && CanSee(enemy, newMoveTarget, LayerMask.GetMask("Default")); i++)
                //newMoveTarget = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
                newMoveTarget = grid.getRandomAccessibleGridNode();
        }
        target.position = newMoveTarget;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (path == null || path.Count == 0) 
            return;

        foreach (var point in path)
            Gizmos.DrawSphere(point, 0.15f);

        for (int i = 0; i < path.Count - 1; i++)
        {
            Gizmos.DrawLine(path[i], path[i + 1]);
        }

        if (currentWaypoint < path.Count)
            Gizmos.DrawLine(transform.position, path[currentWaypoint]);
    }
}
