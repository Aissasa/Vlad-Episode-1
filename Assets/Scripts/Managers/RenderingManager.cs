using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderingManager : MonoBehaviour
{
    protected static RenderingManager instance;

    public static RenderingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RenderingManager();
            }
            return instance;
        }
    }

    private RenderingManager() { }

    List<GameObject> characters = new List<GameObject>();

    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

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

    void RetrieveCharacters()
    {
        characters.Add(GameObject.FindGameObjectWithTag("Player"));
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            characters.Add(enemy);
        }
    }
}
