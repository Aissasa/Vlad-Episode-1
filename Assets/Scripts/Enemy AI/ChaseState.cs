using UnityEngine;
using System.Collections;
using System;

namespace EnemyAI
{
    public class ChaseState : IEnemyState
    {
        protected readonly EnemyStateHandler enemy;
        protected EnemyMovementHandler movementHandler;

        protected float pathRefreshTimer;

        public ChaseState(EnemyStateHandler enemyState)
        {
            enemy = enemyState;
            movementHandler = new EnemyMovementHandler(enemy.gameObject);
            pathRefreshTimer = GameManager.Instance.pathFindingRate;
        }

        public void DrawGizmos()
        {
            movementHandler.DrawPath();
        }

        public void ResetVariables()
        {
            movementHandler.ResetToZero();
            pathRefreshTimer = GameManager.Instance.pathFindingRate;
        }

        public void ToAttackState()
        {
            ResetVariables();
            enemy.currentEnemyState = enemy.attackState;
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
            if (PlayerInAttackRange())
            {
                enemy.anim.SetBool(enemy.walkingAnimationBool, false);
                ToAttackState();
            }
            if (!PlayerInPursuitRange())
            {
                enemy.anim.SetBool(enemy.walkingAnimationBool, false);
                ToLookOutState();
            }
        }

        protected void Chase()
        {
            if (movementHandler.PathEmpty() || pathRefreshTimer <= 0)
            {
                PathRequestManager.RequestPath(enemy.transform.position, enemy.player.Get2DPosition(), enemy.gameObject, enemy.unwalkableLayer, OnPathFound);
                pathRefreshTimer = GameManager.Instance.pathFindingRate;
            }
            else
            {
                pathRefreshTimer -= Time.deltaTime;
            }
            FlipIfNeeded();
            if (enemy.GetAnimationState() != EnemyStateHandler.MyAnimationState.Walk)
            {
                enemy.anim.SetBool(enemy.walkingAnimationBool, true);
            }
            //enemy.animator.SetBool(enemy.walkingAnimationBool, true);
            movementHandler.MoveAlongPath();
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
                movementHandler.Reset(enemy.gameObject, newPath, enemy.characterSpeed);
        }

        protected bool PlayerInAttackRange()
        {
            return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.player.Get2DPosition()) <= enemy.attackRange;
        }

        protected bool PlayerInPursuitRange()
        {
            return DirectionAndDistanceCalculator.CalculateDistance(enemy.transform.Get2DPosition(), enemy.player.Get2DPosition()) <= enemy.pursuitRange;
        }
    }
}