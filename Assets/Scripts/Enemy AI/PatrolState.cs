using UnityEngine;
using System.Collections;
using System;

namespace EnemyAI
{
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

        public void DrawGizmos()
        {
            movementHandler.DrawPath();
        }

        public void ResetVariables()
        {
            movementHandler.ResetToZero();
            patrolWaitTimer = 0;
            nextWaypointIndex = 0;
        }

        public void ToAttackState()
        {
            Debug.Log("Cant go from patrol to attack directly");
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

        protected void FlipIfNeeded()
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

        // todo : random next patrol point
        protected void Patrol()
        {
            if (movementHandler.PathEmpty())
                PathRequestManager.RequestPath(enemy.transform.position, enemy.patrolWaypoints[nextWaypointIndex].Get2DPosition(), enemy.gameObject, enemy.unwalkableLayer, OnPathFound);
            if (patrolWaitTimer > 0)
            {
                patrolWaitTimer -= Time.deltaTime;
                return;
            }

            FlipIfNeeded();
            enemy.anim.SetBool(enemy.walkingAnimationBool, true);
            movementHandler.MoveAlongPath();
            if (DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.patrolWaypoints[nextWaypointIndex].Get2DPosition()) < GameManager.Instance.aiReachingPrecision)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % enemy.patrolWaypoints.Length;
                PathRequestManager.RequestPath(enemy.transform.position, enemy.patrolWaypoints[nextWaypointIndex].Get2DPosition(), enemy.gameObject, enemy.unwalkableLayer, OnPathFound);
                patrolWaitTimer = enemy.patrolDelay;
                enemy.anim.SetBool(enemy.walkingAnimationBool, false);
            }
        }

        protected bool PlayerInChasingRange()
        {
            return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.player.Get2DPosition()) <= enemy.chasingRange;
        }

    }
}