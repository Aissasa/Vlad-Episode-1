using UnityEngine;
using System.Collections;
using System;

public class EnemyController : GenericCharacterController
{
    public bool displayPathGizmos;

    Transform player;
    Vector2[] path;
    int targetIndex;
    Vector2 nextPos;

    protected override void Start()
    {
        base.Start();
        isFacingRight = false;
        characterSpeed = 1.5f;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        PathRequestManager.RequestPath(transform.position, player.position, OnPathFound);

    }

    public void OnPathFound(Vector2[] newPath, bool pathFound)
    {
        if (pathFound)
        {
            path = newPath;
            StopCoroutine(Follow());
            StartCoroutine(Follow());
        }
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

    protected IEnumerator Follow()
    {
        Vector2 currentWayPoint = path[0];
        nextPos = currentWayPoint;
        movementVector = CalculateDirection(transform.position, path[targetIndex]);
        Vector2 enemyPosition = new Vector2();
        while (true)
        {
            enemyPosition.x = transform.position.x;
            enemyPosition.y = transform.position.y;
            if (currentWayPoint==enemyPosition)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                movementVector = CalculateDirection(path[targetIndex-1], path[targetIndex]);
                currentWayPoint = path[targetIndex];
                nextPos = currentWayPoint;
            }
            MoveCharacter();
            yield return null;
        }

    }

    protected Vector2 CalculateDirection(Vector2 startPos, Vector2 targetPos)
    {
        return new Vector2(targetPos.x - startPos.x, targetPos.y - startPos.y);
    }

    protected override void Move()
    {
        LinearMouvement.Instance().MoveTo(gameObject ,nextPos, characterSpeed);
        //Debug.Log("MovementVect"+ movementVector);
        //LinearMouvement.Instance().Move(gameObject, movementVector, characterSpeed);
    }

    void OnDrawGizmos()
    {
        if (path != null && displayPathGizmos)
        {
            //Vector2 init = transform.position;
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(new Vector3(path[i].x, path[i].y, 0.6f), new Vector3(0.1f, 0.1f, 0.1f));
                //if (i==1)
                //{
                //    Gizmos.DrawCube(new Vector3(init.x, init.y, 0.6f), new Vector3(0.1f, 0.1f, 0.1f));
                //    Gizmos.DrawLine(init, path[i]);
                //}
                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }
}
