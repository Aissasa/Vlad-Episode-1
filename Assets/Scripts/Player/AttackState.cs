using UnityEngine;
using System.Collections;
using System;

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
            player.currentPlayerState = player.idleState;
        }

        public void ToMoveState()
        {
            player.currentPlayerState = player.idleState;
        }

        public void UpdateState()
        {
            Attack();
            if (!player.isAttacking)
            {
                player.anim.ResetTrigger(player.attackingAnimationTrigger);
                if (Vector2.zero == player.movementVector)
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
            Debug.Log("YAAAAA!!!");
            player.anim.SetTrigger(player.attackingAnimationTrigger);
        }
    }
}

