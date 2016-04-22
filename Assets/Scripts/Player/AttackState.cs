﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace PlayerLogic
{
    public class AttackState : IPlayerState
    {

        protected readonly PlayerStateHandler player;

        public AttackState(PlayerStateHandler playerState)
        {
            player = playerState;
        }

        public void ResetVariables()
        {
        }

        public void ToAttackState()
        {
            Debug.Log("Cant go to the same state");
        }

        public void ToIdleState()
        {
            player.CurrentPlayerState = player.IdleState;
        }

        public void ToMoveState()
        {
            player.CurrentPlayerState = player.IdleState;
        }

        public void UpdateState()
        {
            Attack();
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

        protected void Attack()
        {
            if (!player.IsAttacking || player.InBlockingAnimation())
            {
                return;
            }
            player.Anim.SetTrigger(player.AttackingAnimationTrigger);
            List<GameObject> hitCharacters = GetSurroundingDamageableCharacters(); // new add getbreakableobjects with unwalkable layer and then breakable tag
            hitCharacters.Remove(player.gameObject);
            Debug.Log(hitCharacters.Count);
            foreach (var character in hitCharacters)
            {
                character.GetComponent<IDamageable>().TakeDamage(player.PlayerStats);
            }

            // todo : lookat nearest enemy
        }
        // urgent : think of facing nearest enemy when attacking
        protected List<GameObject> GetSurroundingDamageableCharacters()
        {
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

