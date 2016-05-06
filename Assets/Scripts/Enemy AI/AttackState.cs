using UnityEngine;
using System.Collections;
using System;

namespace EnemyAI
{
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
            //Debug.Log("Nothing to draw in attack");
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
            enemy.CurrentEnemyState = enemy.ChaseState;
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
            if (!PlayerInAttackRange() && enemy.GetAnimationState() != EnemyStateHandler.MyAnimationState.Attack)
            {
                ToChaseState();
            }
        }

        protected void Attack()
        {
            if (attackTimer <= 0)
            {
                enemy.Anim.SetTrigger(enemy.AttackingAnimationTrigger);
                //SoundManager
                attackTimer = enemy.AttackDelay;
            }
            else
            {
                attackTimer -= Time.deltaTime;
                if (!enemy.InBlockingAnimation())
                {
                    LookAtPlayer();
                }
            }
        }

        protected void LookAtPlayer()
        {
            if (enemy.transform.Get2DPosition().IsAtLeftOf(enemy.Player.Get2DPosition()) && !enemy.IsFacingRight ||
                !enemy.transform.Get2DPosition().IsAtLeftOf(enemy.Player.Get2DPosition()) && enemy.IsFacingRight)
            {
                enemy.Flip();
            }
        }

        protected bool PlayerInAttackRange()
        {
            return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.Player.Get2DPosition()) <= enemy.attackRange;
        }

    }
}