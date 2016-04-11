using UnityEngine;
using System.Collections;
using System;

public class ChaseState : IEnemyState
{
    public void OntriggerEnter(Collider other)
    {
        throw new NotImplementedException();
    }

    public void ToChaseState()
    {
        Debug.Log("Cant go to the same state");
    }

    public void ToLookOutState()
    {
        throw new NotImplementedException();
    }

    public void ToPatrolState()
    {
        Debug.Log("Cant go to from chase to patrol directly");
    }

    public void UpdateState()
    {
        throw new NotImplementedException();
    }
}
