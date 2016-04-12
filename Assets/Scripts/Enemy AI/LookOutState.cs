using UnityEngine;
using System.Collections;
using System;

public class LookOutState : IEnemyState
{
    protected readonly EnemyStateHandler enemy;
    protected float lookOutTimer;

    public LookOutState(EnemyStateHandler enemyState)
    {
        enemy = enemyState;
        ResetVariables();
    }

    public void ResetVariables()
    {
        lookOutTimer = enemy.lookOutDelay;
    }

    public void ToChaseState()
    {
        ResetVariables();
        enemy.currentEnemyState = enemy.chaseState;
    }

    public void ToLookOutState()
    {
        Debug.Log("Cant go to the same state");
    }

    public void ToPatrolState()
    {
        ResetVariables();
        enemy.currentEnemyState = enemy.patrolState;
    }

    public void UpdateState()
    {
        if (PlayerInChasingRange())
        {
            ToChaseState();
        }
        if (lookOutTimer<= 0)
        {
            ToPatrolState();
        }
        lookOutTimer -= Time.deltaTime;
    }

    protected bool PlayerInChasingRange()
    {
        return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.player.Get2DPosition()) <= enemy.chasingRange;
    }

}
