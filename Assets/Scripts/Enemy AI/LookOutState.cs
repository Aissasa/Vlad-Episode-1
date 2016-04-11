using UnityEngine;
using System.Collections;
using System;

public class LookOutState : IEnemyState
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
        Debug.Log("Cant go to the same state");
    }

    public void ToPatrolState()
    {
        throw new NotImplementedException();
    }

    public void UpdateState()
    {
        throw new NotImplementedException();
    }
}
