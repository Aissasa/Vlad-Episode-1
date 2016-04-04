using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EnemyController : GenericCharacterController
{
    public bool displayPathGizmos;
    public bool refineIt;
    public bool smoothIt;
    public bool smoothItFurther;

    [Range(0, 1)]
    public float bezierInterpolationRange;
    public LayerMask unwalkableLayer;

    Transform player;
    Vector2[] path;
    BezierPath bezierPath;
    int targetIndex;
    Vector2 nextPos;
    //bool isChasing;

    protected override void Start()
    {
        base.Start();
        if (GetComponent<SpriteRenderer>().flipX)
            isFacingRight = false;
        characterSpeed = 1f;
        //isChasing = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);

    }

    protected override void AttackFunc()
    {
    }

    protected override void GotHit()
    {
    }

    protected override void GotKilled()
    {
    }

    protected override void Move()
    {
        //ps : test
        //Vector2 test;
        //ObstacleFinder.Instance.CheckObstacles(gameObject, transform.position, nextPos, unwalkableLayer, out test);
        LinearMouvement.Instance().MoveTo(gameObject, nextPos, characterSpeed);
        //LinearMouvement.Instance().MoveTowards(gameObject, nextPos - transform.Get2DPosition());
    }

    protected IEnumerator Follow()
    {
        Vector2 currentWayPoint = path[0];
        nextPos = currentWayPoint;
        movementVector = DirectionAndDistanceCalculator.CalculateSignedDirection(transform.position, path[targetIndex]);
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
                movementVector = DirectionAndDistanceCalculator.CalculateSignedDirection(path[targetIndex - 1], path[targetIndex]);
                currentWayPoint = path[targetIndex];
                nextPos = currentWayPoint;
            }
            MoveCharacter();
            if (transform.Get2DPosition().Equals(path[path.Length - 1]))
            {
                movementVector = Vector2.zero;
            }
            yield return null;
        }
    }

    public void OnPathFound(Vector2[] newPath, bool pathFound)
    {
        if (pathFound)
        {
            path = newPath;
            if (refineIt)
            {
                RefinePath();
            }
            if (smoothIt)
            {
                SmoothPath();
            }
            if (smoothItFurther)
            {
                BezierInterpolate();
                Debug.Log(path.Length);
            }

            StopCoroutine(Follow());
            StartCoroutine(Follow());
        }
    }

    protected void BezierInterpolate()
    {
        bezierPath = new BezierPath();
        List<Vector2> thePath = new List<Vector2>(path);
        bezierPath.Interpolate(thePath, bezierInterpolationRange);

        path = bezierPath.GetPathPoints().ToArray();

    }


    protected void SmoothPath()
    {
        Vector2 checkPoint = path[0];
        Vector2 currentPoint;
        Vector2 colliderPosition;
        List<Vector2> smootherPath = new List<Vector2>();
        smootherPath.Add(checkPoint);

        for (int i = 2; i < path.Length; i++)
        {
            currentPoint = path[i];
            if (ObstacleFinder.Instance.CheckObstacles(gameObject, checkPoint, currentPoint, unwalkableLayer, out colliderPosition))
            {
                checkPoint = path[i - 1];
                smootherPath.Add(checkPoint);
            }
        }

        smootherPath.Add(path[path.Length - 1]);
        path = smootherPath.ToArray();
    }

    protected void RefinePath()
    {
        List<Vector2> refinedPath = new List<Vector2>(path);
        Vector2 colliderPosition;

        int i = 1;
        while (i < refinedPath.Count)
        {
            Vector2 startPos = refinedPath[i - 1];
            Vector2 endPos = refinedPath[i];
            Vector2 direction = DirectionAndDistanceCalculator.CalculateSignedDirection(startPos, endPos);
            if (direction.x != 0 && direction.y !=0)
            {
                if (ObstacleFinder.Instance.CheckObstacles(gameObject, startPos, endPos, unwalkableLayer, out colliderPosition))
                {
                    Vector2 middle = DirectionAndDistanceCalculator.GetMiddleOfVector(startPos, endPos);
                    Vector2 newPoint;
                    if (colliderPosition.IsAbove(middle))
                    {
                        if (direction.y>0)
                        {
                            newPoint = new Vector2(endPos.x, startPos.y);
                        }
                        else
                        {
                            newPoint = new Vector2(startPos.x, endPos.y);
                        }
                    }
                    else
                    {
                        if (direction.y>0)
                        {
                            newPoint = new Vector2(startPos.x, endPos.y);
                        }
                        else
                        {
                            newPoint = new Vector2(endPos.x, startPos.y);
                        }
                    }

                    refinedPath.Insert(i, newPoint);
                }
            }

            i++;
        }

        path = refinedPath.ToArray();
    }

    void OnDrawGizmos()
    {
        if (path != null && displayPathGizmos)
        {
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(new Vector3(path[i].x, path[i].y, 0.6f), new Vector3(0.1f, 0.1f, 0.1f));
                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
