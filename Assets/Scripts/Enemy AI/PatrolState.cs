using UnityEngine;
using System.Collections;
using System;

public class PatrolState : IEnemyState
{
    public void OntriggerEnter(Collider other)
    {
        throw new NotImplementedException();
    }

    public void ToChaseState()
    {
        throw new NotImplementedException();
    }

    public void ToLookOutState()
    {
        Debug.Log("Cant go from patrol to lookout directly");

    }

    public void ToPatrolState()
    {
        Debug.Log("Cant go to the same state");
    }

    public void UpdateState()
    {
        throw new NotImplementedException();
    }
}
