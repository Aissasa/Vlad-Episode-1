using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

    public Transform target;
    float speed = 1;
    Vector2[] path;
    int targetIndex;

    void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector2[] newPath, bool pathFound)
    {
        if (pathFound)
        {
            path = newPath;
            StopCoroutine("Follow");
            StartCoroutine("Follow");
        }
    }

    IEnumerator Follow()
    {
        Vector2 currentWayPoint = path[0];
        //targetIndex = 0;

        Vector2 enemyPosition = new Vector2();
        while (true)
        {
            enemyPosition.x = transform.position.x;
            enemyPosition.y = transform.position.y;
            if (currentWayPoint == enemyPosition)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWayPoint = path[targetIndex];
            }

            transform.position = Vector2.MoveTowards(transform.position, currentWayPoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        if (path != null )
        {
            for (int i = 0; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(new Vector3(path[i].x, path[i].y, 0.6f), new Vector3(0.1f, 0.1f, 0.1f));

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                //else
                //{
                //    Gizmos.DrawLine(path[i - 1], path[i]);
                //}
            }
        }
    }
}
