using UnityEngine;
using System.Collections.Generic;

public class GridNode
{
    public Vector2 Position;
    public bool IsAccessible;

    public GridNode Parent;
    public float G; // Cost from start to current
    public float H; // Heuristic cost from current to goal
    public float F => G + H;

    public List<GridNode> Neighbors = new List<GridNode>();

    public GridNode(Vector2 position, bool accessible)
    {
        Position = position;
        IsAccessible = accessible;
    }
}
