using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    //note : test
    public bool displayGridGizmos = false;
    //public Transform playerPosition;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
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
                bool walkable = !Physics2D.CircleCast(worldPoint, nodeRadius, Vector2.zero, Mathf.Infinity, unwalkableMask);
                grid[x, y] = new Node(worldPoint, walkable, x, y);
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
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) // in bounds
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbors;
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
            //Node playerNode = NodeFromWorldPoint(playerPosition.position);
            foreach (var node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.blue;
                //Gizmos.color = node == playerNode ? Color.cyan : Gizmos.color;
                Gizmos.DrawCube(new Vector3(node.worldPosition.x, node.worldPosition.y, 0.5f),
                    new Vector3(nodeDiameter - nodeRadius / 5, nodeDiameter - nodeRadius / 5, 0.1f));
            }
        }
    }
}
