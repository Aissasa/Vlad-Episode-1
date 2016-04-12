using UnityEngine;
using System.Collections;
using System;

public class PatrolState : IEnemyState
{
    protected readonly EnemyStateHandler enemy;
    protected int nextWaypointIndex;
    protected EnemyMovementHandler movementHandler;
    protected float patrolWaitTimer;

    public PatrolState(EnemyStateHandler enemyState)
    {
        enemy = enemyState;
        movementHandler = new EnemyMovementHandler(enemy.gameObject);
        patrolWaitTimer = 0;
        nextWaypointIndex = 0;
    }

    public void ResetVariables()
    {
        movementHandler.ResetToZero();
        patrolWaitTimer = 0;
        nextWaypointIndex = 0;
    }

    public void ToChaseState()
    {
        ResetVariables();
        enemy.currentEnemyState = enemy.chaseState;
    }

    public void ToLookOutState()
    {
        Debug.Log("Cant go from patrol to lookout directly");
    }

    public void ToPatrolState()
    {
        Debug.Log("Cant go to the same state");
    }

    public void UpdateState()
    {
        Patrol();
        if (PlayerInChasingRange())
        {
            ToChaseState();
        }
    }

    protected void FlipIfPossible()
    {
        Vector2 direction = movementHandler.GetMovementDirection();
        if ((direction.x > 0 && !enemy.isFacingRight) || (direction.x < 0 && enemy.isFacingRight))
        {
            enemy.Flip();
        }
    }

    protected void OnPathFound(Vector2[] newPath, bool pathFound)
    {
        if (pathFound)
        {
            movementHandler.Reset(enemy.gameObject, newPath, enemy.characterSpeed);
        }
    }

    protected void Patrol()
    {
        if (movementHandler.PathEmpty())
            PathRequestManager.RequestPath(enemy.transform.position, enemy.patrolWaypoints[nextWaypointIndex].Get2DPosition(), enemy.gameObject, enemy.unwalkableLayer, enemy.bezierInterpolationRange, OnPathFound);
        if (patrolWaitTimer > 0)
        {
            patrolWaitTimer -= Time.deltaTime;
            return;
        }

        FlipIfPossible();
        enemy.animator.SetBool(enemy.walkingAnimationBool, true);
        movementHandler.MoveAlongPath();
        // new add the 0.1 value to global values
        if (DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.patrolWaypoints[nextWaypointIndex].Get2DPosition()) < 0.15f)
        {
            nextWaypointIndex = (nextWaypointIndex + 1) % enemy.patrolWaypoints.Length;
            PathRequestManager.RequestPath(enemy.transform.position, enemy.patrolWaypoints[nextWaypointIndex].Get2DPosition(), enemy.gameObject, enemy.unwalkableLayer, enemy.bezierInterpolationRange, OnPathFound);
            patrolWaitTimer = enemy.inPatrolPointDelay;
            enemy.animator.SetBool(enemy.walkingAnimationBool, false);
        }
    }

    protected bool PlayerInChasingRange()
    {
        return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.player.Get2DPosition()) <= enemy.chasingRange;
    }
}
