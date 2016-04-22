using UnityEngine;
using System.Collections;
using System;

namespace EnemyAI
{
    public class LookOutState : IEnemyState
    {
        protected readonly EnemyStateHandler enemy;
        protected float lookOutTimer;

        public LookOutState(EnemyStateHandler enemyState)
        {
            enemy = enemyState;
            ResetVariables();
        }

        public void DrawGizmos()
        {
            Debug.Log("Nothing to draw in lookout");
        }

        public void ResetVariables()
        {
            lookOutTimer = enemy.LookOutDelay;
        }

        public void ToAttackState()
        {
            Debug.Log("Cant go to from lookout to attack directly");
        }

        public void ToChaseState()
        {
            ResetVariables();
            enemy.CurrentEnemyState = enemy.ChaseState;
        }

        public void ToLookOutState()
        {
            Debug.Log("Cant go to the same state");
        }

        public void ToPatrolState()
        {
            ResetVariables();
            enemy.CurrentEnemyState = enemy.PatrolState;
        }

        public void UpdateState()
        {
            if (PlayerInChasingRange())
            {
                ToChaseState();
            }
            if (lookOutTimer <= 0)
            {
                ToPatrolState();
            }
            lookOutTimer -= Time.deltaTime;
        }

        protected bool PlayerInChasingRange()
        {
            return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.Player.Get2DPosition()) <= enemy.chasingRange;
        }

    }
}