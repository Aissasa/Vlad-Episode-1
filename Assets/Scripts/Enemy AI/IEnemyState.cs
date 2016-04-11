using UnityEngine;
using System.Collections;

public interface IEnemyState {

    void UpdateState();

    void OntriggerEnter(Collider other);

    void ToPatrolState();

    void ToChaseState();

    void ToLookOutState();

}
