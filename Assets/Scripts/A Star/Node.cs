using UnityEngine;
using System.Collections;
using System;

public class Node : IHeapItem<Node>
{

    public Vector2 worldPosition { get; set; }
    public bool walkable { get; set; }
    public int gridX { get; set; }
    public int gridY { get; set; }

    public int gCost { get; set; } // distance from starting node
    public int hCost { get; set; } // disrance from target node (heuristic)
    public int fCost { get { return gCost + hCost; } }

    public Node parent { get; set; }

    public Node(Vector2 _worldPosition, bool _walkable = false, int _gridX = 0, int _gridY = 0)
    {
        worldPosition = _worldPosition;
        walkable = _walkable;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int heapIndex { get; set; }

    public int CompareTo(Node node)
    {
        int compare = fCost.CompareTo(node.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(node.hCost);
        }

        return compare;
    }
}
