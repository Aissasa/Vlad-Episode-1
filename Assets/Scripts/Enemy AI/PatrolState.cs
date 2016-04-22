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
            enemy.CurrentEnemyState = enemy.ChaseState;
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
            if ((direction.x > 0 && !enemy.IsFacingRight) || (direction.x < 0 && enemy.IsFacingRight))
            {
                enemy.Flip();
            }
        }

        protected void OnPathFound(Vector2[] newPath, bool pathFound)
        {
            if (pathFound)
            {
                movementHandler.Reset(enemy.gameObject, newPath, enemy.CharacterSpeed);
            }
        }

        // todo : random next patrol point
        protected void Patrol()
        {
            if (movementHandler.PathEmpty())
                PathRequestManager.RequestPath(enemy.transform.position, enemy.PatrolWayPoints[nextWaypointIndex].Get2DPosition(), enemy.gameObject, enemy.UnwalkableLayer, OnPathFound);
            if (patrolWaitTimer > 0)
            {
                patrolWaitTimer -= Time.deltaTime;
                return;
            }

            FlipIfNeeded();
            enemy.Anim.SetBool(enemy.WalkingAnimationBool, true);
            movementHandler.MoveAlongPath();
            if (DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.PatrolWayPoints[nextWaypointIndex].Get2DPosition()) < GameManager.Instance.aiReachingPrecision)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % enemy.PatrolWayPoints.Length;
                PathRequestManager.RequestPath(enemy.transform.position, enemy.PatrolWayPoints[nextWaypointIndex].Get2DPosition(), enemy.gameObject, enemy.UnwalkableLayer, OnPathFound);
                patrolWaitTimer = enemy.PatrolDelay;
                enemy.Anim.SetBool(enemy.WalkingAnimationBool, false);
            }
        }

        protected bool PlayerInChasingRange()
        {
            return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.Player.Get2DPosition()) <= enemy.chasingRange;
        }

    }
}