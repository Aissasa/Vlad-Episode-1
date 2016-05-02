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
            player.Anim.SetBool(player.WalkingAnimationBool, false);
            player.CurrentPlayerState = player.AttackState;
        }

        public void ToIdleState()
        {
            player.Anim.SetBool(player.WalkingAnimationBool, false);
            player.CurrentPlayerState = player.IdleState;
        }

        public void ToMoveState()
        {
            Debug.Log("Cant go to the same state");
        }

        public void ToRollState()
        {
            player.Anim.SetBool(player.WalkingAnimationBool, true);
            player.CurrentPlayerState = player.RollState;
        }

        public void UpdateState()
        {
            MovePlayer();
            if (player.IsAttacking)
            {
                ToAttackState();
            }
            if (Vector2.zero == player.MovementVector)
            {
                ToIdleState();
            }
            if (Vector2.zero != player.PositionToRollTo)
            {
                ToRollState();
            }
        }

        protected void FlipIfNeeded()
        {
            if ((player.MovementVector.x > 0 && !player.IsFacingRight) || (player.MovementVector.x < 0 && player.IsFacingRight))
            {
                player.Flip();
            }

        }

        protected void MovePlayer()
        {
            FlipIfNeeded();
            player.Anim.SetBool(player.WalkingAnimationBool, true);
            //ps : change this to keys if needed
            LinearMouvement.Instance.MoveTowardsWithJoyStick(player.gameObject, player.MovementVector, player.PlayerSpeed);
        }
    }
}

