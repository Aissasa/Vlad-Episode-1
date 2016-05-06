using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace PlayerLogic
{
    public class AttackState : IPlayerState
    {
        public List<GameObject> AttackTargets { get; set; }

        protected readonly PlayerStateHandler player;

        public AttackState(PlayerStateHandler playerState)
        {
            player = playerState;
        }

        public void ResetVariables()
        {
            AttackTargets = null;
        }

        public void ToAttackState()
        {
            Debug.Log("Cant go to the same state");
        }

        public void ToIdleState()
        {
            ResetVariables();
            player.CurrentPlayerState = player.IdleState;
        }

        public void ToMoveState()
        {
            ResetVariables();
            player.CurrentPlayerState = player.IdleState;
        }

        public void ToRollState()
        {
            Debug.Log("Cant go to the roll state from attack state");
        }

        public void UpdateState()
        {
            LaunchAttack();
            if (!player.IsAttacking)
            {
                player.Anim.ResetTrigger(player.AttackingAnimationTrigger);
                if (Vector2.zero == player.MovementVector)
                {
                    ToIdleState();
                }
                else
                {
                    ToMoveState();
                }
            }
        }

        protected bool IsFacing(Vector2 playerPos, Vector2 enemyPos)
        {
            float angle = DirectionAndDistanceCalculator.CalculateAngle(playerPos, enemyPos);
            float playerDirection = player.IsFacingRight ? 0 : Mathf.PI; // this will depend on the blend tree

            float rawfinal = playerDirection - angle; 
            float final = Mathf.Abs(rawfinal);
            if (Mathf.Abs(rawfinal - 2 * Mathf.PI) < final)
            {
                final = Mathf.Abs(rawfinal - 2 * Mathf.PI);
            }
            return final < Mathf.PI / 2;
        }

        protected void LaunchAttack()
        {
            if (!player.IsAttacking || player.InBlockingAnimation())
            {
                return;
            }

            player.Anim.SetTrigger(player.AttackingAnimationTrigger);
            SoundManager.instance.RandomizeCombatSfx(true, player.SwooshSound1, player.SwooshSound2);
            AttackTargets = GetSurroundingDamageableCharacters(); // new add getbreakableobjects with unwalkable layer and then breakable tag
            AttackTargets.Remove(player.gameObject);
            LookAtNearestEnemy(AttackTargets);

            // new : change this when adding the other directions, aka vlad
            GameObject[] stub = AttackTargets.ToArray();
            foreach (var enemy in stub)
            {
                if (!IsFacing(player.transform.Get2DPosition(), enemy.transform.Get2DPosition()))
                {
                    AttackTargets.Remove(enemy);
                }
            }
        }

        protected List<GameObject> GetSurroundingDamageableCharacters()
        {
            RaycastHit2D[] array = Physics2D.CircleCastAll(player.transform.Get2DPosition(), player.PlayerAttackRange, Vector2.zero, Mathf.Infinity, player.DamageableLayer);
            List<GameObject> gos = new List<GameObject>();
            foreach (var item in array)
            {
                gos.Add(item.transform.gameObject);
            }

            return gos;
        }

        protected void LookAtNearestEnemy(List<GameObject> enemies)
        {
            if (enemies.Count < 1)
            {
                return;
            }

            float minDistance = DirectionAndDistanceCalculator.CalculateDistance(player.transform.Get2DPosition(), enemies[0].transform.Get2DPosition());
            int nearestEnemyIndex = 0;

            for (int i = 1; i < enemies.Count; i++)
            {
                if (DirectionAndDistanceCalculator.CalculateDistance(player.transform.Get2DPosition(), enemies[i].transform.Get2DPosition()) < minDistance)
                {
                    minDistance = DirectionAndDistanceCalculator.CalculateDistance(player.transform.Get2DPosition(), enemies[i].transform.Get2DPosition());
                    nearestEnemyIndex = i;
                }
            }

            Vector2 enemyPos = enemies[nearestEnemyIndex].transform.Get2DPosition();

            if ((player.transform.Get2DPosition().IsAtLeftOf(enemyPos) && !player.IsFacingRight) || (!player.transform.Get2DPosition().IsAtLeftOf(enemyPos) && player.IsFacingRight))
            {
                player.Flip();
            }
        }
    }
}

