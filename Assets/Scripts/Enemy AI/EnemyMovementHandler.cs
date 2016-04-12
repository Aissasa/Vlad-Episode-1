using UnityEngine;
using System.Collections;
using System;

public class EnemyMovementHandler {

    public GameObject gameObject { get; set; }
    public Vector2[] path { get; set; }
    public int targetIndex { get; set; }
    public Vector2 currentWayPoint { get; set; }
    public float charSpeed { get; set; }

    public EnemyMovementHandler(GameObject go)
    {
        gameObject = go;
        targetIndex = 0;
    }

    public void DrawPath()
    {
        if (!PathEmpty())
        {
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(new Vector3(path[i].x, path[i].y, 0.6f), new Vector3(0.1f, 0.1f, 0.1f));
                if (i == targetIndex)
                {
                    Gizmos.DrawLine(gameObject.transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    public Vector2 GetMovementDirection()
    {
        return DirectionAndDistanceCalculator.CalculateSignedDirection(gameObject.transform.Get2DPosition(), currentWayPoint);
    }

    public void MoveAlongPath()
    {
        if (path == null || path.Length <= 0)
        {
            return;
        }

        LinearMouvement.Instance.MoveToPosition(gameObject, currentWayPoint, charSpeed);
        if (gameObject.transform.Get2DPosition() == currentWayPoint)
        {
            targetIndex++;
            if (targetIndex >= path.Length)
            {
                path = null;
                return;
            }
            currentWayPoint = path[targetIndex];
        }
    }

    public void Reset(GameObject go, Vector2[] _path, float speed)
    {
        if (_path == null || _path.Length <= 0)
        {
            Debug.Log("path null");
            return;
        }
        gameObject = go;
        path = _path;
        charSpeed = speed;
        targetIndex = 0;
        currentWayPoint = path[targetIndex];
    }

    public void ResetToZero()
    {
        path = null;
        targetIndex = 0;
    }

    public bool PathEmpty()
    {
        return path == null || path.Length <= 0;
    }

}
