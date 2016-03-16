using UnityEngine;
using System.Collections;

public class Warper : MonoBehaviour {

    public Transform target_point;
    string _screen_fader_tag = "ScreenFader";

    void Start()
    {
        //if (target_point == null) //just if we want to ensure the presence of target point
    }

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        // stop any mouvement and animations
        other.gameObject.GetComponent<PlayerControl>().enabled = false;
        other.gameObject.GetComponent<Animator>().enabled = false;

        ScreenFader screenFader = GameObject.FindGameObjectWithTag(_screen_fader_tag).GetComponent<ScreenFader>();
        yield return StartCoroutine(screenFader.FadeOutCoroutine());

        //warp
        other.gameObject.transform.position = target_point.position;
        Camera.main.transform.position = target_point.position;

        yield return StartCoroutine(screenFader.FadeInCoroutine());

        // enable mouvements and animations
        other.gameObject.GetComponent<Animator>().enabled = true;
        other.gameObject.GetComponent<PlayerControl>().enabled = true;


    }

}
