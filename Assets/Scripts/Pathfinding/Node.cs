using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Node : IHeapItem<Node>
{

    public Vector2 WorldPosition { get; set; }
    public bool Walkable { get; set; }
    public int GridX { get; set; }
    public int GridY { get; set; }

    public int GCost { get; set; } // distance from starting node
    public int HCost { get; set; } // disrance from target node (heuristic)
    public int FCost { get { return GCost + HCost; } }

    public Node Parent { get; set; }
    public List<Node> IllegalNodes { get; set; } 

    public Node(Vector2 _worldPosition, bool _walkable = false, int _gridX = 0, int _gridY = 0, List<Node> _illegalNodes = null)
    {
        WorldPosition = _worldPosition;
        Walkable = _walkable;
        GridX = _gridX;
        GridY = _gridY;
        IllegalNodes = _illegalNodes;
    }

    public int HeapIndex { get; set; }

    public int CompareTo(Node node)
    {
        int compare = FCost.CompareTo(node.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(node.HCost);
        }

        return -compare;
    }
}
