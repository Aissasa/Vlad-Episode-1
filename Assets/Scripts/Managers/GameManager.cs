using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    private GameManager() { }

    [HideInInspector]
    public float bezierInterpolationRange;

    [HideInInspector]
    public float pathFindingRate;      // refresh rate for pathfinding

    [HideInInspector] // todo : maybe it should be added to the enemy rather than global
    public float aiReachingPrecision; //the precision of reaching a point by the AI when walking

    void DeadEnemy(GameObject go)
    {
        Destroy(go, 5);
    }

    void DeadPlayer(GameObject go)
    {
        // todo : add player death logic here
    }

    void OnEnable()
    {
        PlayerLogic.PlayerStateHandler.DeadPlayer += DeadPlayer;
        EnemyAI.EnemyStateHandler.DeadEnemy += DeadEnemy;
    }

    void OnDisable()
    {
        PlayerLogic.PlayerStateHandler.DeadPlayer -= DeadPlayer;
        EnemyAI.EnemyStateHandler.DeadEnemy -= DeadEnemy;
    }


}
