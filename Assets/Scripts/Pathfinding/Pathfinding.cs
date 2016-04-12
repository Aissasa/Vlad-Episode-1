using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    //public Transform playerPosition;

    PathRequestManager requestManager;
    List<Node> shortestPath;
    GameObject currentGameObject;
    LayerMask currentLayerMask;
    float currentBezierInterpolationRange;

    Grid grid;
    int diagonalCost = 14, lateralCost = 10;

    void Awake()
    {
        grid = GetComponent<Grid>();
        //if (playerPosition == null)
        //{
        //    playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        //}
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartPathFinding(Vector2 startPos, Vector2 targetPos, GameObject go, LayerMask layerMask, float bezierInterpolationRange)
    {
        currentGameObject = go;
        currentLayerMask = layerMask;
        currentBezierInterpolationRange = bezierInterpolationRange;
        Vector2 collider;
        if (!ObstacleFinder.Instance.CheckObstacles(currentGameObject, currentGameObject.transform.Get2DPosition(), targetPos, currentLayerMask, out collider))
        {
            UnityEngine.Debug.Log("Direct with object " + currentGameObject.name);
            requestManager.FinishedProcessingPath(new Vector2[] { targetPos }, true);
        }
        else
            StartCoroutine(FindPath(startPos, targetPos));
    }

    Vector2 CalculateDirection(Node startNode, Node targetNode)
    {
        return new Vector2(startNode.gridX - targetNode.gridX, startNode.gridY - targetNode.gridY);
    }


    IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Vector2[] wayPoints = new Vector2[0];
        bool pathFindingSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
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
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newMouvementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                    if (newMouvementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newMouvementCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;

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
            wayPoints = PathRefiner.Instance.RefineAndSmoothPath(currentGameObject, wayPoints, currentLayerMask, currentBezierInterpolationRange);
        }
        // note : test
        List<Vector2> list = new List<Vector2>(wayPoints);
        list.Add(targetPos);
        requestManager.FinishedProcessingPath(list.ToArray(), pathFindingSuccess);
    }

    int GetDistance(Node node1, Node node2)
    {
        int distX = Mathf.Abs(node1.gridX - node2.gridX);
        int distY = Mathf.Abs(node1.gridY - node2.gridY);

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
            currentNode = currentNode.parent;
        }
        return ReverseAndConvert(shortestPath);
        //return SimplifyAndReversePath(shortestPath);
    }

    private Vector2[] ReverseAndConvert(List<Node> path)
    {
        List<Vector2> wayPoints = new List<Vector2>();

        foreach (var node in path)
        {
            wayPoints.Add(node.worldPosition);
        }

        wayPoints.Reverse();
        return wayPoints.ToArray();
    }

    // todo : delete this
    Vector2[] SimplifyAndReversePath(List<Node> path)
    {
        List<Vector2> wayPoints = new List<Vector2>();
        // note :test
        //wayPoints.Add(path[0].worldPosition);
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = CalculateDirection(path[i - 1], path[i]);
            if (directionNew != directionOld)
            {
                wayPoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        wayPoints.Reverse();
        return wayPoints.ToArray();
    }
}
