using UnityEngine;
using System.Collections;

public interface IEnemyState {

    void ToPatrolState();

    void ToChaseState();

    void ToLookOutState();

    void UpdateState();

    void ResetVariables();

}
