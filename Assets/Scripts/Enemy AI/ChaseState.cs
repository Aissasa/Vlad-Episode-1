using UnityEngine;
using System.Collections;
using System;

public class ChaseState : IEnemyState
{
    protected readonly EnemyStateHandler enemy;
    protected EnemyMovementHandler movementHandler;

    protected float pathRefreshTimer;

    public ChaseState(EnemyStateHandler enemyState)
    {
        enemy = enemyState;
        movementHandler = new EnemyMovementHandler(enemy.gameObject);
        pathRefreshTimer = enemy.pathRefreshDelay; // urgent : externalize game wide variables to a game manager which will be a singleton
    }

    public void ResetVariables()
    {
        pathRefreshTimer = enemy.pathRefreshDelay;

    }

    public void ToChaseState()
    {
        Debug.Log("Cant go to the same state");
    }

    public void ToLookOutState()
    {
        ResetVariables();
        enemy.currentEnemyState = enemy.lookOutState;
    }

    public void ToPatrolState()
    {
        Debug.Log("Cant go to from chase to patrol directly");
    }

    public void UpdateState()
    {
        Chase();
        if (!PlayerInPursuitRange())
        {
            enemy.animator.SetBool(enemy.walkingAnimationBool, false);
            ToLookOutState();
        }
    }

    protected void Chase()
    {
        if (movementHandler.PathEmpty() || pathRefreshTimer <= 0)
        {
            PathRequestManager.RequestPath(enemy.transform.position, enemy.player.Get2DPosition(), enemy.gameObject, enemy.unwalkableLayer, enemy.bezierInterpolationRange, OnPathFound);
            pathRefreshTimer = enemy.pathRefreshDelay;
        }
        else
        {
            pathRefreshTimer -= Time.deltaTime;
        }
        FlipIfPossible();
        if (enemy.GetAnimationState() != EnemyStateHandler.MyAnimationState.Walk)
        {
            enemy.animator.SetBool(enemy.walkingAnimationBool, true);
        }
        //enemy.animator.SetBool(enemy.walkingAnimationBool, true);
        movementHandler.MoveAlongPath();
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
            movementHandler.Reset(enemy.gameObject, newPath, enemy.characterSpeed);
    }

    protected bool PlayerInPursuitRange()
    {
        return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.player.Get2DPosition()) <= enemy.pursuitRange;
    }
}
