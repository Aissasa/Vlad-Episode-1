using UnityEngine;
using System.Collections;
using PlayerLogic;
using EnemyAI;
using System;
using System.Collections.Generic;

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

    public bool PlayerIsDead { get; private set; }

    public float GameTimer { get; set; }

    [SerializeField]
    private GameObject currentMap; // todo: search with tag better, and add event to change it in warp
    public GameObject CuurentMap { get { return currentMap; } set { currentMap = value; } }

    [SerializeField]
    private LayerMask unwalkableLayer;
    public LayerMask UnwalkableLayer { get { return unwalkableLayer; } }

    void Awake()
    {
        PlayerGO = GameObject.FindGameObjectWithTag("Player");
        PlayerIsDead = false;
        GameTimer = 0;
    }

    void Update()
    {
        GameTimer += Time.deltaTime;
    }

    public Health GetCurrentPlayerHealth()
    {
        return new Health(PlayerGO.GetComponent<PlayerStateHandler>().PlayerStats);
    }

    public Vector2 GetMapBottomLeftPosition()
    {
        return currentMap.transform.Get2DPosition() - currentMap.GetComponentInChildren<Grid>().GridWorldSize / 2;
    }

    public Vector2 GetMapTopRightPosition()
    {
        return currentMap.transform.Get2DPosition() + currentMap.GetComponentInChildren<Grid>().GridWorldSize / 2;
    }

    void DeadEnemy(GameObject go)
    {
        go.layer = LayerMask.NameToLayer("DeadCharacters");
        Destroy(go, 5);
    }

    void DeadPlayer(GameObject go)
    {
        // todo : add player death logic here
        PlayerIsDead = true;
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
