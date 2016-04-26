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

        protected void LaunchAttack()
        {
            if (!player.IsAttacking || player.InBlockingAnimation())
            {
                return;
            }

            player.Anim.SetTrigger(player.AttackingAnimationTrigger);
            AttackTargets = GetSurroundingDamageableCharacters(); // new add getbreakableobjects with unwalkable layer and then breakable tag
            AttackTargets.Remove(player.gameObject);

            //foreach (var character in hitCharacters)
            //{
            //    character.GetComponent<IDamageable>().TakeDamage(player.PlayerStats);
            //}

            // todo : lookat nearest enemy
        }
        // urgent : think of facing nearest enemy when attacking
        protected List<GameObject> GetSurroundingDamageableCharacters()
        {
            //urgent : think attack vector
            //ps : test
            //RaycastHit2D[] array = Physics2D.CircleCastAll(player.transform.Get2DPosition(), 0.2f, player.movementVector, Mathf.Infinity, player.damageableLayer);
            RaycastHit2D[] array = Physics2D.CircleCastAll(player.transform.Get2DPosition(), player.PlayerAttackRange, Vector2.zero, Mathf.Infinity, player.DamageableLayer);
            List<GameObject> gos = new List<GameObject>();
            foreach (var item in array)
            {
                gos.Add(item.transform.gameObject);
            }

            return gos;
        }

        // new :  think about player direction in raycasting
        protected bool IsFacingEnemy(Transform enemy)
        {
            return true;
        }

    }
}

