using System.Collections.Generic;
using UnityEngine;

public class PathfindingGrid : MonoBehaviour
{
    [SerializeField] private int gridSize = 10;
    [SerializeField] private float nodeSpacing = 1.0f;
    [SerializeField] private LayerMask obstacleLayer;

    private GridNode[,] nodes;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        nodes = new GridNode[gridSize, gridSize];

        Vector2 bottomLeft = (Vector2)transform.position - new Vector2(gridSize, gridSize) * nodeSpacing * 0.5f;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector2 pos = bottomLeft + new Vector2(x, y) * nodeSpacing;
                bool accessible = !Physics2D.OverlapCircle(pos, nodeSpacing * 0.4f, obstacleLayer);
                nodes[x, y] = new GridNode(pos, accessible);
            }
        }

        // Set neighbors
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                nodes[x, y].Neighbors = GetNeighbors(nodes[x, y]);
            }
        }
    }

    List<GridNode> GetNeighbors(GridNode node)
    {
        List<GridNode> neighbors = new List<GridNode>();
        foreach (Vector2 dir in new Vector2[] {
                     Vector2.up, Vector2.down, Vector2.left, Vector2.right,
                     new Vector2(1,1), new Vector2(-1,1), new Vector2(-1,-1), new Vector2(1,-1)})
        {
            Vector2 neighborPos = node.Position + dir * nodeSpacing;
            GridNode neighbor = GetNodeAtPosition(neighborPos);

            if (neighbor != null && neighbor.IsAccessible && !IsBlockedByObstacle(node, neighbor))
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    GridNode GetNodeAtPosition(Vector2 pos)
    {
        foreach (var node in nodes)
        {
            if (Vector2.Distance(node.Position, pos) < nodeSpacing / 2)
                return node;
        }
        return null;
    }

    bool IsBlockedByObstacle(GridNode from, GridNode to)
    {
        RaycastHit2D hit = Physics2D.Linecast(from.Position, to.Position, obstacleLayer);
        return hit.collider != null;
    }

    GridNode FindClosestNode(Vector2 pos)
    {
        GridNode closest = null;
        float minDist = float.MaxValue;
        foreach (var node in nodes)
        {
            float dist = Vector2.Distance(pos, node.Position);
            if (dist < minDist && node.IsAccessible)
            {
                minDist = dist;
                closest = node;
            }
        }
        return closest;
    }

    public List<Vector2> GetPath(Transform seeker, Vector2 targetPos)
    {
        GridNode startNode = FindClosestNode(seeker.position);
        GridNode targetNode = FindClosestNode(targetPos);

        if (startNode == null || targetNode == null)
            return null;

        List<GridNode> openSet = new List<GridNode> { startNode };
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        startNode.G = 0;
        startNode.H = Vector2.Distance(startNode.Position, targetNode.Position);

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => a.F.CompareTo(b.F));
            GridNode currentNode = openSet[0];

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (GridNode neighbor in currentNode.Neighbors)
            {
                if (closedSet.Contains(neighbor)) continue;

                float tentativeG = currentNode.G + Vector2.Distance(currentNode.Position, neighbor.Position);
                if (tentativeG < neighbor.G || !openSet.Contains(neighbor))
                {
                    neighbor.G = tentativeG;
                    neighbor.H = Vector2.Distance(neighbor.Position, targetNode.Position);
                    neighbor.Parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        return null; // no path found
    }

    List<Vector2> RetracePath(GridNode startNode, GridNode endNode)
    {
        List<Vector2> path = new List<Vector2>();
        GridNode current = endNode;

        while (current != startNode)
        {
            path.Add(current.Position);
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        foreach (var node in nodes)
        {
            Gizmos.color = node.IsAccessible ? Color.white : Color.red;
            Gizmos.DrawWireCube(node.Position, Vector3.one * nodeSpacing * 0.9f);
        }
    }

    public Vector2 getRandomAccessibleGridNode()
    {
        GridNode node;
        do
        {
            int x = Random.Range(0, gridSize);
            int y = Random.Range(0, gridSize);
            node = nodes[x, y];
        } while (!node.IsAccessible);
        return node.Position;
        
    }
}
