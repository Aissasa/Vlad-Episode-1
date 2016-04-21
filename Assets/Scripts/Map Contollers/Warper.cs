using UnityEngine;
using System.Collections;

public class Warper : MonoBehaviour {

    public Transform targetPoint;
    string screenFaderTag = "ScreenFader";

    //void Start()
    //{
    //    if (target_point == null) //just if we want to ensure the presence of target point
    //}

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        // stop any mouvement and animations
        //other.gameObject.GetComponent<PlayerController>().enabled = false;
        other.gameObject.GetComponent<PlayerLogic.PlayerStateHandler>().enabled = false;
        other.gameObject.GetComponent<Animator>().enabled = false;

        ScreenFader screenFader = GameObject.FindGameObjectWithTag(screenFaderTag).GetComponent<ScreenFader>();
        yield return StartCoroutine(screenFader.FadeOutCoroutine());

        //warp
        other.gameObject.transform.position = targetPoint.position;
        Camera.main.transform.position = targetPoint.position;

        yield return StartCoroutine(screenFader.FadeInCoroutine());

        // enable mouvements and animations
        other.gameObject.GetComponent<Animator>().enabled = true;
        //other.gameObject.GetComponent<PlayerController>().enabled = true;
        other.gameObject.GetComponent<PlayerLogic.PlayerStateHandler>().enabled = true;

    }

}
