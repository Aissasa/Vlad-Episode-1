using UnityEngine;
using System.Collections;

namespace EnemyAI
{
    public interface IEnemyState
    {

        void DrawGizmos();

        void ResetVariables();

        void ToAttackState();

        void ToPatrolState();

        void ToChaseState();

        void ToLookOutState();

        void UpdateState();

    }
}
