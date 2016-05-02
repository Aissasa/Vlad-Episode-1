using UnityEngine;
using System.Collections;


namespace PlayerLogic
{
    public interface IPlayerState
    {
        void ResetVariables(); // todo : maybe i don't need this

        void ToAttackState();

        void ToIdleState();

        void ToMoveState();

        void ToRollState();

        void UpdateState();

    }
}

