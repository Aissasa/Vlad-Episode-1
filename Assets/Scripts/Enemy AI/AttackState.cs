using UnityEngine;
using System.Collections;
using System;

public class AttackState : IEnemyState
{
    protected readonly EnemyStateHandler enemy;
    protected float attackTimer;

    public AttackState(EnemyStateHandler enemyState)
    {
        enemy = enemyState;
        ResetVariables();
    }

    public void DrawGizmos()
    {
        Debug.Log("Nothing to draw in attack");
    }

    public void ResetVariables()
    {
        attackTimer = 0;
    }

    public void ToAttackState()
    {
        Debug.Log("Cant go to the same state");
    }

    public void ToChaseState()
    {
        ResetVariables();
        enemy.currentEnemyState = enemy.chaseState;
    }

    public void ToLookOutState()
    {
        Debug.Log("Cant go from attack to lookout directly");
    }

    public void ToPatrolState()
    {
        Debug.Log("Cant go from attack to patrol directly");
    }

    public void UpdateState()
    {
        Attack();
        if (!PlayerInAttackRange() && enemy.GetAnimationState()!= EnemyStateHandler.MyAnimationState.Attack)
        {
            ToChaseState();
        }
    }

    protected void Attack()
    {
        if (attackTimer<=0)
        {
            Debug.Log("THIS IS SPARTAAAAA !!!!");
            enemy.animator.SetTrigger(enemy.attackingAnimationTrigger);
            attackTimer = enemy.attackDelay;
            // urgent add attack logic here for enemies
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }
    }

    protected bool PlayerInAttackRange()
    {
        return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.player.Get2DPosition()) <= enemy.attackRange;
    }

}
