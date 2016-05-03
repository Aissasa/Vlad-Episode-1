using UnityEngine;
using System.Collections;
using System;

namespace PlayerLogic
{
    public class IdleState : IPlayerState
    {

        protected readonly PlayerStateHandler player;

        public IdleState(PlayerStateHandler playerState)
        {
            player = playerState;
        }

        public void ResetVariables()
        {
        }

        public void ToAttackState()
        {
            player.CurrentPlayerState = player.AttackState;
        }

        public void ToIdleState()
        {
            Debug.Log("Cant go to the same state");
        }

        public void ToMoveState()
        {
            player.CurrentPlayerState = player.MoveState;
        }

        public void ToRollState()
        {
            player.Anim.SetBool(player.WalkingAnimationBool, true);
            player.CurrentPlayerState = player.RollState;
        }

        public void UpdateState()
        {
            if (Vector2.zero != player.MovementVector)
            {
                ToMoveState();
            }
            if (player.IsAttacking)
            {
                ToAttackState();
            }
            if (Vector2.zero != player.NextPositionToRollTo)
            {
                ToRollState();
            }

        }
    }
}

