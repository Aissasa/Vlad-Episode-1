using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{


    Transform player;
    Vector3 firstPlayerPosition;
    Vector3 cameraSpacer; // to insure z is not 0

    Camera cam;
    float camScale;
    float camLerpSpeed;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        firstPlayerPosition = GameObject.FindGameObjectWithTag("FirstPosition").GetComponent<Transform>().position;

        cam = GetComponent<Camera>();
        cameraSpacer = new Vector3(0, 0, -10);

        transform.position = firstPlayerPosition + cameraSpacer;
        camScale = 3f;
        camLerpSpeed = 0.1f;
    }

    void Update()
    {
        //adjust the camera to the resolution of the screen
        cam.orthographicSize = (Screen.height / 100f) / camScale;
        if (player)
        {
            // smooth camera transition following the player
            transform.position = Vector3.Lerp(transform.position, player.position, camLerpSpeed) + cameraSpacer;
        }
    }
}
