using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{


    Transform _player;
    Camera _cam;
    [SerializeField]
    float _cam_scale;

    [SerializeField]
    float _cam_lerp_speed;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _cam = GetComponent<Camera>();

        transform.position = _player.position;
        _cam_scale = 3f;
        _cam_lerp_speed = 0.1f;
    }

    void Update()
    {
        //adjust the camera to the resolution of the screen
        _cam.orthographicSize = (Screen.height / 100f) / _cam_scale;
        if (_player)
            // smooth camera transition following the player
            transform.position = Vector3.Lerp(transform.position, _player.position, _cam_lerp_speed) + new Vector3(0, 0, -10);
    }
}
