using UnityEngine;
using System.Collections;
using System;

namespace PlayerLogic
{
    public class RollState : IPlayerState
    {
        protected readonly PlayerStateHandler player;

        protected Vector2 rollTargetPosition;
        protected float rollTimer;

        public RollState(PlayerStateHandler playerState)
        {
            player = playerState;
            rollTargetPosition = Vector2.zero;
            rollTimer = player.RollDuration;
        }


        public void ResetVariables()
        {
            player.Anim.SetBool(player.WalkingAnimationBool, false);
            player.PositionToRollTo = Vector2.zero;
            rollTargetPosition = Vector2.zero;
            rollTimer = player.RollDuration;
            player.PlayerStats.DodgeRate -= player.RollDodgeBuff;

        }

        public void ToAttackState()
        {
            ResetVariables();
            player.CurrentPlayerState = player.AttackState;
        }

        public void ToIdleState()
        {
            ResetVariables();
            player.CurrentPlayerState = player.IdleState;
        }

        public void ToMoveState()
        {
            ResetVariables();
            player.CurrentPlayerState = player.MoveState;
        }

        public void ToRollState()
        {
            Debug.Log("Cant go to the same state");
        }

        public void UpdateState()
        {
            if (rollTimer == player.RollDuration)
            {
                player.PlayerStats.DodgeRate += player.RollDodgeBuff;
            }
            if (rollTimer > 0 && !CheckObstables())
            {
                Roll();
            }
            else
            {
                ToIdleState();
            }
        }

        protected void Roll()
        {
            LinearMouvement.Instance.MoveTo(player.gameObject, player.PositionToRollTo, player.PlayerSpeed * player.RollBurst);
            player.InvokeAfterImage();
            rollTimer -= Time.deltaTime;
        }

        protected bool CheckObstables()
        {
            Vector2 castDirection = DirectionAndDistanceCalculator.CalculateDirection(player.transform.Get2DPosition(), rollTargetPosition);
            return  Physics2D.CircleCast(player.transform.Get2DPosition(), 0.15f, castDirection, 0.25f, GameManager.Instance.UnwalkableLayer);
        }
    }
}