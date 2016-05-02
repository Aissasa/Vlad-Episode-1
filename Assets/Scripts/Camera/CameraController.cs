using UnityEngine;
using System.Collections;
using EnemyAI;


public class CameraController : MonoBehaviour
{
    [Range(1, 10)]
    [SerializeField]
    private float camScale;
    [Range(0.05f, 1)]
    [SerializeField]
    private float camLerpSpeed;

    [Space(10)]
    [Range(0, 2)]
    [SerializeField]
    private float horizontalCamBuffer;
    [Range(0, 2)]
    [SerializeField]
    private float verticalCamBuffer;


    [Space(10)]
    [Range(0, 2)]
    [SerializeField]
    private float shakeDuration;
    [Range(0, 2)]
    [SerializeField]
    private float shakeMagnitude;
    [Range(1, 5)]
    [SerializeField]
    private float shakeSpeed;

    Camera cam;
    Vector3 playerFirstPosition;
    Vector3 cameraSpacer; // to insure z is not 0

    Vector2 mapTopRight;
    Vector2 mapBottomLeft;
    bool outLeft;
    bool outRight;
    bool outTop;
    bool outButtom;

    void Awake()
    {
        playerFirstPosition = GameObject.FindGameObjectWithTag("PlayerFirstPosition").GetComponent<Transform>().position;
        cam = GetComponent<Camera>();
        cameraSpacer = new Vector3(0, 0, -10);

        transform.position = playerFirstPosition + cameraSpacer;

        mapTopRight = GameManager.Instance.GetMapTopRightPosition();
        mapBottomLeft = GameManager.Instance.GetMapBottomLeftPosition();

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

    void OnEnable()
    {
        EnemyStateHandler.HitEnemy += ShakeForHit;
    }

    void OnDisable()
    {
        EnemyStateHandler.HitEnemy -= ShakeForHit;
    }

    private void ShakeForHit()
    {
        StopAllCoroutines();
        StartCoroutine(Shake());
    }

    private void ResetCamPosition()
    {
        if (GameManager.PlayerGO)
        {
            Transform player = GameManager.PlayerGO.transform;
            // smooth camera transition following the player
            Vector3 buffer = Vector3.Lerp(transform.position, player.position, camLerpSpeed) + cameraSpacer;
            Vector2 camButtomLeft = cam.BoundsMin();
            Vector2 camTopRight = cam.BoundsMax();

            if (!outLeft) // if cam is not blocked out left
            {
                if (camButtomLeft.x <= mapBottomLeft.x - horizontalCamBuffer) // test if the left edge of the cam surpassed the map's
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
                if (camButtomLeft.y <= mapBottomLeft.y - verticalCamBuffer)
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
                if (camTopRight.x >= mapTopRight.x + horizontalCamBuffer)
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
                if (camTopRight.y >= mapTopRight.y + verticalCamBuffer)
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

    IEnumerator Shake()
    {
        float elapsed = 0.0f;

        Vector3 originalCamPos = cam.transform.position;
        float randomStart = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        while (elapsed < shakeDuration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / shakeDuration;

            // to reduce the shaking magnitude from full power to 0, beginning from halfway the shaking duration
            float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);

            float alpha = randomStart +  shakeSpeed * percentComplete;

            // map value to [-1, 1], that smoothly change thanks to Perlin noise
            float x = SimplexNoise.Noise(alpha, 0);
            float y = SimplexNoise.Noise(0, alpha);
            //float x = Mathf.PerlinNoise(alpha, 0);
            //float y = Mathf.PerlinNoise(0, alpha);

            x *= shakeMagnitude * damper;
            y *= shakeMagnitude * damper;

            cam.transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

            yield return null;
        }

        cam.transform.position = originalCamPos;
    }

    private bool PlayerStillOutLeft()
    {
        return GameManager.PlayerGO.transform.Get2DPosition().x < cam.transform.Get2DPosition().x;
    }

    private bool PlayerStillOutRight()
    {
        return GameManager.PlayerGO.transform.Get2DPosition().x > cam.transform.Get2DPosition().x;
    }

    private bool PlayerStillOutTop()
    {
        return GameManager.PlayerGO.transform.Get2DPosition().y > cam.transform.Get2DPosition().y;
    }

    private bool PlayerStillOutBottom()
    {
        return GameManager.PlayerGO.transform.Get2DPosition().y < cam.transform.Get2DPosition().y;
    }

}
