using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EnemyController : GenericCharacterController
{
    public LayerMask unwalkableLayer;

    public bool displayPathGizmos;
    public bool refineIt;
    public bool smoothIt;
    public bool smoothItFurther;

    [Range(0, 1)]
    public float bezierInterpolationRange;
    [HideInInspector]
    public float chasingRange;
    [HideInInspector]
    public float attackRange;


    Transform player;
    Vector2[] path;
    BezierPath bezierPath;
    Vector2 nextPos;
    Vector2 currentWayPoint;
    int targetIndex;
    bool isChasing;
    bool isInAttackRange;

    public void OnPathFound(Vector2[] newPath, bool pathFound)
    {
        if (pathFound)
        {
            //if (path == null || path.Length <= 0)
            //{
            //    path = newPath;
            //}
            //else
            //{
            //    path = AddNewPath(targetIndex, newPath);
            //}
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
            }
            targetIndex = 0;
            currentWayPoint = path[targetIndex];

        }
    }

    protected override void Start()
    {
        base.Start();
        if (GetComponent<SpriteRenderer>().flipX)
            isFacingRight = false;
        isChasing = false;
        isInAttackRange = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    protected override void AttackFunc()
    {
        if (PlayerInAttackRange())
        {
            // todo : stop movement & attack
        }
    }

    protected override void GotHit()
    {
    }

    protected override void GotKilled()
    {
    }

    protected override void Move()
    {
        if (path == null || path.Length<=0)
        {
            return;
        }

        if (PlayerInAttackRange())
        {
            isChasing = false;
            path = null;
            return;
        }

        if (isChasing)
        {
            LinearMouvement.Instance().MoveToPosition(gameObject, currentWayPoint, characterSpeed);
            if (transform.Get2DPosition() == currentWayPoint)
            {
                targetIndex++;
                if (targetIndex>= path.Length)
                {
                    isChasing = false;
                    path = null;
                    return;
                }
                currentWayPoint = path[targetIndex];
            }
        }
    }

    protected Vector2[] AddNewPath(int index ,Vector2[] newPath)
    {
        List<Vector2> oldPath = new List<Vector2>(path);
        if (oldPath == null || oldPath.Count<=0 )
        {
            return null;
        }
        for (int i = 0; i < index; i++)
        {
            oldPath.RemoveAt(0);
        }
        oldPath.AddRange(newPath);
        return oldPath.ToArray();
    }

    protected void BezierInterpolate()
    {
        bezierPath = new BezierPath();
        List<Vector2> thePath = new List<Vector2>(path);
        bezierPath.Interpolate(thePath, bezierInterpolationRange);

        path = bezierPath.GetPathPoints().ToArray();

    }

    protected void Chase()
    {
        if (PlayerInAttackRange())
        {
            return;
        }
        if (PlayerInChasingRange())
        {
            Vector2 collider;
            if (!ObstacleFinder.Instance.CheckObstacles(gameObject, transform.Get2DPosition(), player.transform.Get2DPosition(), unwalkableLayer, out collider))
                OnPathFound(new Vector2[] { player.transform.Get2DPosition() }, true);
            else
                PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);

            isChasing = true;
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                path = null;
            }
        }
    }

    protected void OnEnable()
    {
        PlayerController.Moved += Chase;
    }

    protected void OnDisable()
    {
        PlayerController.Moved -= Chase;
    }

    protected void OnDrawGizmos()
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

    protected bool PlayerInAttackRange()
    {
        return DirectionAndDistanceCalculator.CalculateDistance(transform.Get2DPosition(), player.Get2DPosition()) <= attackRange;
    }

    protected bool PlayerInChasingRange()
    {
        return DirectionAndDistanceCalculator.CalculateDistance(transform.Get2DPosition(), player.Get2DPosition()) <= chasingRange;
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
            if (direction.x != 0 && direction.y != 0)
            {
                if (ObstacleFinder.Instance.CheckObstacles(gameObject, startPos, endPos, unwalkableLayer, out colliderPosition))
                {
                    Vector2 middle = DirectionAndDistanceCalculator.GetMiddleOfVector(startPos, endPos);
                    Vector2 newPoint;
                    if (colliderPosition.IsAbove(middle))
                    {
                        if (direction.y > 0)
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
                        if (direction.y > 0)
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

    protected void SmoothPath()
    {
        if (path == null)
        {
            return;
        }
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

    protected override void UpdateMovementVector()
    {
        if (!isChasing)
        {
            movementVector = Vector2.zero;
        }
        else
        {
            movementVector = DirectionAndDistanceCalculator.CalculateSignedDirection(transform.Get2DPosition(), currentWayPoint);
        }
    }
}
