using UnityEngine;
using System.Collections;
using Tiled2Unity;
using System;

public class CameraController : MonoBehaviour
{
    public GameObject currentMap; // todo: search with tag better, and add event to change it in warp

    [Range(1, 10)]
    [SerializeField]
    private float camScale;
    [Range(0.05f, 1)]
    [SerializeField]
    private float camLerpSpeed;

    [Range(0, 2)]
    [SerializeField]
    private float horizontalCamBuffer;
    [Range(0, 2)]
    [SerializeField]
    private float verticalCamBuffer;


    Transform player;
    Vector3 playerFirstPosition;
    Vector3 cameraSpacer; // to insure z is not 0

    Vector2 mapTopLeft;
    Vector2 mapBottomRight;
    bool outLeft;
    bool outRight;
    bool outTop;
    bool outButtom;

    Camera cam;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerFirstPosition = GameObject.FindGameObjectWithTag("PlayerFirstPosition").GetComponent<Transform>().position;
        cam = GetComponent<Camera>();
        cameraSpacer = new Vector3(0, 0, -10);

        transform.position = playerFirstPosition + cameraSpacer;

        mapTopLeft = currentMap.GetComponent<TiledMap>().GetMapTopLeftPos();
        mapBottomRight = currentMap.GetComponent<TiledMap>().GetMapBottomRightPos();
        outLeft = false;
        outRight = false;
        outTop = false;
        outButtom = false;
    }

    void LateUpdate()
    {
        //adjust the camera to the resolution of the screen
        cam.orthographicSize = (Screen.height / 100f) / camScale;
        ResetCamPosition();
    }

    private void ResetCamPosition()
    {
        if (player)
        {
            // smooth camera transition following the player
            Vector3 buffer = Vector3.Lerp(transform.position, player.position, camLerpSpeed) + cameraSpacer;
            Vector2 camButtomLeft = cam.BoundsMin();
            Vector2 camTopRight = cam.BoundsMax();

            if (!outLeft) // if cam is not blocked out left
            {
                if (camButtomLeft.x <= mapTopLeft.x - horizontalCamBuffer) // test if the left edge of the cam surpassed the map's
                {
                    outLeft = true;                 // then the cam is outleft
                    buffer.x = transform.position.x; // so we block it
                }
            }
            else // the cam is already stopped at left
            {
                if (PlayerStillOutLeft()) // if player is still at left
                {
                    buffer.x = transform.position.x; // keep the cam at left
                }
                else // the player is not at left
                {
                    outLeft = false; // then stop blocking the cam
                }
            }

            if (!outButtom)
            {
                if (camButtomLeft.y <= mapBottomRight.y - verticalCamBuffer)
                {
                    outButtom = true;
                    buffer.y = transform.position.y;
                }
            }
            else
            {
                if (PlayerStillOutBottom())
                {
                    buffer.y = transform.position.y;
                }
                else
                {
                    outButtom = false;
                }
            }

            if (!outRight)
            {
                if (camTopRight.x >= mapBottomRight.x + horizontalCamBuffer)
                {
                    outRight = true;
                    buffer.x = transform.position.x;
                }
            }
            else
            {
                if (PlayerStillOutRight())
                {
                    buffer.x = transform.position.x;
                }
                else
                {
                    outRight = false;
                }
            }

            if (!outTop)
            {
                if (camTopRight.y >= mapTopLeft.y + verticalCamBuffer)
                {
                    outTop = true;
                    buffer.y = transform.position.y;
                }
            }
            else
            {
                if (PlayerStillOutTop())
                {
                    buffer.y = transform.position.y;
                }
                else
                {
                    outTop = false;
                }
            }

            transform.position = buffer;
        }

    }

    private bool PlayerStillOutLeft()
    {
        return player.Get2DPosition().x < cam.transform.Get2DPosition().x;
    }

    private bool PlayerStillOutRight()
    {
        return player.Get2DPosition().x > cam.transform.Get2DPosition().x;
    }

    private bool PlayerStillOutTop()
    {
        return player.Get2DPosition().y > cam.transform.Get2DPosition().y;
    }

    private bool PlayerStillOutBottom()
    {
        return player.Get2DPosition().y < cam.transform.Get2DPosition().y;
    }

}
