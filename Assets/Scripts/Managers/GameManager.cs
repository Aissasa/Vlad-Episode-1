using UnityEngine;
using System.Collections;
using PlayerLogic;
using EnemyAI;

public class GameManager : Singleton<GameManager>
{
    private GameManager() { }

    [HideInInspector]
    public float bezierInterpolationRange;

    [HideInInspector]
    public float pathFindingRate;      // refresh rate for pathfinding

    [HideInInspector] // todo : maybe it should be added to the enemy rather than global
    public float aiReachingPrecision; //the precision of reaching a point by the AI when walking

    public static GameObject PlayerGO;

    void Awake()
    {
        PlayerGO = GameObject.FindGameObjectWithTag("Player");
    }

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
        PlayerStateHandler.DeadPlayer += DeadPlayer;
        EnemyStateHandler.DeadEnemy += DeadEnemy;
    }

    void OnDisable()
    {
        PlayerStateHandler.DeadPlayer -= DeadPlayer;
        EnemyStateHandler.DeadEnemy -= DeadEnemy;
    }


}
