using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [Range(1, 10)]
    public float camScale;
    [Range(0.05f, 1)]
    public float camLerpSpeed;

    Transform player;
    Vector3 playerFirstPosition;
    Vector3 cameraSpacer; // to insure z is not 0

    Camera cam;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerFirstPosition = GameObject.FindGameObjectWithTag("PlayerFirstPosition").GetComponent<Transform>().position;
        cam = GetComponent<Camera>();
        cameraSpacer = new Vector3(0, 0, -10);

        transform.position = playerFirstPosition + cameraSpacer;
        //camScale = 3f;
        //camLerpSpeed = 0.1f;
    }

    void LateUpdate()
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
