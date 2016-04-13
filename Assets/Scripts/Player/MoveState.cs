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
            player.currentPlayerState = player.attackState;
        }

        public void ToIdleState()
        {
            player.currentPlayerState = player.idleState;
        }

        public void ToMoveState()
        {
            Debug.Log("Cant go to the same state");
        }

        public void UpdateState()
        {
            if (player.CanMove())
            {
                MovePlayer();
            }
            if (player.isAttacking)
            {
                ToAttackState();
            }
            if (Vector2.zero == player.movementVector)
            {
                player.anim.SetBool(player.walkingAnimationBool, false);
                ToIdleState();
            }
        }

        protected void FlipIfPossible()
        {
            if ((player.movementVector.x > 0 && !player.isFacingRight) || (player.movementVector.x < 0 && player.isFacingRight))
            {
                player.Flip();
            }

        }

        protected void MovePlayer()
        {
            FlipIfPossible();
            player.anim.SetBool(player.walkingAnimationBool, true);
            LinearMouvement.Instance.MoveTowardsDirection(player.gameObject, player.movementVector, player.playerSpeed);
        }
    }
}

