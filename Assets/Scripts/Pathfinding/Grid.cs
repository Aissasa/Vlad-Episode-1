using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    //ps : test
    [SerializeField]
    private bool displayGridGizmos = false;
    [SerializeField]
    private bool showAll = false;
    [SerializeField]
    private LayerMask unwalkableMask;
    [SerializeField]
    private Vector2 gridWorldSize;
    public Vector2 GridWorldSize { get { return gridWorldSize; } }
    [SerializeField]
    [Range(0.01f, 5)]
    private float nodeRadius = 0.15f;
    Node[,] grid;

    Vector2 worldBottomLeft;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    // for the heap size
    public int MaxSize { get { return gridSizeX * gridSizeY; } }

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        worldBottomLeft = new Vector2() + Vector2.left * gridWorldSize.x / 2 + Vector2.down * gridWorldSize.y / 2;
        CreateGrid();
        illegalizeNodes();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius)
                    + Vector2.up * (y * nodeDiameter + nodeRadius);
                //bool walkable = !Physics2D.CircleCast(worldPoint, nodeRadius + nodeRadius / 10, Vector2.zero, Mathf.Infinity, unwalkableMask);
                bool walkable = !Physics2D.BoxCast(worldPoint, new Vector2(nodeRadius + nodeRadius/10 , nodeRadius + nodeRadius / 10), 0f, Vector2.zero, Mathf.Infinity, unwalkableMask);
                grid[x, y] = new Node(worldPoint, walkable, x, y, new List<Node>());
            }
        }
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.GridX + x;
                int checkY = node.GridY + y;
                
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) // in bounds
                {
                    if (node.IllegalNodes.Contains(grid[checkX, checkY]) && checkX>0 && checkY>0)
                    {
                        continue;
                    }
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    void illegalizeNodes()
    {
        foreach (var node in grid)
        {
            if (node.GridX <= 0 || node.GridY <= 0 || node.GridX >= gridSizeX -1 || node.GridY >= gridSizeY -1)
            {
                continue;
            }
            if (node.Walkable)
            {
                if (!grid[node.GridX, node.GridY+1].Walkable)
                {
                    if (!grid[node.GridX +1, node.GridY].Walkable)
                    {
                        node.IllegalNodes.Add(grid[node.GridX + 1, node.GridY + 1]);
                    }
                    if (!grid[node.GridX - 1, node.GridY].Walkable)
                    {
                        node.IllegalNodes.Add(grid[node.GridX - 1, node.GridY + 1]);
                    }
                }

                if (!grid[node.GridX, node.GridY - 1].Walkable)
                {
                    if (!grid[node.GridX + 1, node.GridY].Walkable)
                    {
                        node.IllegalNodes.Add(grid[node.GridX + 1, node.GridY - 1]);
                    }
                    if (!grid[node.GridX - 1, node.GridY].Walkable)
                    {
                        node.IllegalNodes.Add(grid[node.GridX - 1, node.GridY - 1]);
                    }
                }
            }
        }
    }

    public Node NodeFromWorldPoint(Vector2 worldPoint)
    {
        float percentX = (worldPoint.x - worldBottomLeft.x) / gridWorldSize.x;
        float percentY = (worldPoint.y - worldBottomLeft.y) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));
        if (grid != null && displayGridGizmos)
        {
            foreach (var node in grid)
            {
                if (showAll)
                {
                    Gizmos.color = node.Walkable ? Color.white : Color.blue;
                    Gizmos.DrawCube(new Vector3(node.WorldPosition.x, node.WorldPosition.y, 0.5f),
                        new Vector3(nodeDiameter - nodeRadius / 5, nodeDiameter - nodeRadius / 5, 0.1f));
                }
                else
                {
                    if (!node.Walkable)
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawCube(new Vector3(node.WorldPosition.x, node.WorldPosition.y, 0.5f),
                        new Vector3(nodeDiameter - nodeRadius / 5, nodeDiameter - nodeRadius / 5, 0.1f));
                    }
                }
            }
        }
    }
}
