using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    List<Node> shortestPath;
    GameObject currentGameObject;
    LayerMask currentLayerMask;

    Grid grid;
    int diagonalCost = 14, lateralCost = 10;

    void Awake()
    {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartPathFinding(Vector2 startPos, Vector2 targetPos, GameObject go, LayerMask layerMask)
    {
        currentGameObject = go;
        currentLayerMask = layerMask;
        Vector2 collider;
        if (!ObstacleFinder.Instance.CheckObstacles(currentGameObject, currentGameObject.transform.Get2DPosition(), targetPos, currentLayerMask, out collider))
        {
            requestManager.FinishedProcessingPath(new Vector2[] { targetPos }, true);
        }
        else
            StartCoroutine(FindPath(startPos, targetPos));
    }

    Vector2 CalculateDirection(Node startNode, Node targetNode)
    {
        return new Vector2(startNode.GridX - targetNode.GridX, startNode.GridY - targetNode.GridY);
    }


    IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Vector2[] wayPoints = new Vector2[0];
        bool pathFindingSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.Walkable && targetNode.Walkable)
        {

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirstItem();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathFindingSuccess = true;
                    break;
                }

                foreach (var neighbor in grid.GetNeighbors(currentNode))
                {
                    if (!neighbor.Walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMouvementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                    if (newMouvementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.GCost = newMouvementCostToNeighbor;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
            yield return null;
        }
        if (pathFindingSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
            wayPoints = PathRefiner.Instance.RefineAndSmoothPath(currentGameObject, wayPoints, currentLayerMask);
        }
        // note : test 
        //List<Vector2> list = new List<Vector2>(wayPoints);
        //list.Add(targetPos);
        requestManager.FinishedProcessingPath(wayPoints, pathFindingSuccess);
    }

    int GetDistance(Node node1, Node node2)
    {
        int distX = Mathf.Abs(node1.GridX - node2.GridX);
        int distY = Mathf.Abs(node1.GridY - node2.GridY);

        if (distX > distY)
        {
            return diagonalCost * distY + lateralCost * (distX - distY);
        }

        return diagonalCost * distX + lateralCost * (distY - distX);
    }

    Vector2[] RetracePath(Node startNode, Node targetNode)
    {
        shortestPath = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            shortestPath.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        return ReverseAndConvert(shortestPath);
    }

    private Vector2[] ReverseAndConvert(List<Node> path)
    {
        List<Vector2> wayPoints = new List<Vector2>();

        foreach (var node in path)
        {
            wayPoints.Add(node.WorldPosition);
        }

        wayPoints.Reverse();
        return wayPoints.ToArray();
    }
}
