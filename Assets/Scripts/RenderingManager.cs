using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderingManager : Singleton<MonoBehaviour> {

    List<GameObject> characters = new List<GameObject>();

    void Start () {

        RetrieveCharacters();
        ArrangeCharactersRenderingOrder();        
	}
	
	void Update () {
        ArrangeCharactersRenderingOrder();
	}

    void ArrangeCharactersRenderingOrder()
    {
        characters.Sort(PositionComparer.Instance);
        int order = 0;
        foreach (var item in characters)
        {
            item.GetComponent<Renderer>().sortingOrder = order;
            order -- ;

        }
    }

    void RetrieveCharacters()
    {
        characters.Add(GameObject.FindGameObjectWithTag("Player"));
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            characters.Add(enemy);
        }
    }
}
