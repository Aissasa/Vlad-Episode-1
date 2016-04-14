using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RenderingManager : Singleton<RenderingManager>
{
    List<GameObject> characters = new List<GameObject>();

    void Start()
    {
        RetrieveCharacters();
        ArrangeCharactersRenderingOrder();
    }

    void Update()
    {
        ArrangeCharactersRenderingOrder();
    }

    void ArrangeCharactersRenderingOrder()
    {
        characters.Sort(PositionComparer.Instance);
        int order = 0;
        foreach (var item in characters)
        {
            item.GetComponent<Renderer>().sortingOrder = order;
            order--;
        }
    }

    // todo : externalize to game manager
    void DeadEnemy(GameObject go)
    {
        characters.Remove(go);
        Destroy(go, 5);
    }

    void RetrieveCharacters()
    {
        characters.Add(GameObject.FindGameObjectWithTag("Player"));
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            characters.Add(enemy);
        }
    }

    void OnEnable()
    {
        EnemyAI.EnemyStateHandler.Dead += DeadEnemy;
    }

    void OnDisable()
    {
        EnemyAI.EnemyStateHandler.Dead -= DeadEnemy;
    }

}
