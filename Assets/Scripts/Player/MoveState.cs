using UnityEngine;
using System.Collections;
using System;

namespace PlayerLogic
{
    public class MoveState : IPlayerState
    {
        protected readonly PlayerStateHandler player;

        public MoveState(PlayerStateHandler playerState)
        {
            player = playerState;
        }

        public void ResetVariables()
        {
        }

        public void ToAttackState()
        {
            player.anim.SetBool(player.walkingAnimationBool, false);
            player.currentPlayerState = player.attackState;
        }

        public void ToIdleState()
        {
            player.anim.SetBool(player.walkingAnimationBool, false);
            player.currentPlayerState = player.idleState;
        }

        public void ToMoveState()
        {
            Debug.Log("Cant go to the same state");
        }

        public void UpdateState()
        {
            MovePlayer();
            if (player.isAttacking)
            {
                ToAttackState();
            }
            if (Vector2.zero == player.movementVector)
            {
                ToIdleState();
            }
        }

        protected void FlipIfNeeded()
        {
            if ((player.movementVector.x > 0 && !player.isFacingRight) || (player.movementVector.x < 0 && player.isFacingRight))
            {
                player.Flip();
            }

        }

        protected void MovePlayer()
        {
            FlipIfNeeded();
            player.anim.SetBool(player.walkingAnimationBool, true);
            //ps : change this to keys if needed
            LinearMouvement.Instance.MoveTowardsWithJoyStick(player.gameObject, player.movementVector, player.playerSpeed);
        }
    }
}

