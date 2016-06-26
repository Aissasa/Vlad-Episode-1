using UnityEngine;
using System.Collections;
using EnemyAI;

public class CameraShaker : MonoBehaviour
{
    [Range(0, 2)]
    [SerializeField]
    private float shakeDuration;
    [Range(0, 2)]
    [SerializeField]
    private float shakeBaseMagnitude;
    [Range(1, 5)]
    [SerializeField]
    private float shakeSpeed;
    [Range(1, 3)]
    [SerializeField]
    private float critShakeMultiplier;

    [Space(10)]
    [SerializeField]
    ParticleSystem critEffect;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        critEffect.transform.localPosition = new Vector3(0, 0, -transform.position.z);    
    }

    void OnEnable()
    {
        EnemyStateHandler.HitEnemy += ShakeForHit;
        EnemyStateHandler.HitEnemy += CritEffectLaunch;
    }

    void OnDisable()
    {
        EnemyStateHandler.HitEnemy -= ShakeForHit;
        EnemyStateHandler.HitEnemy -= CritEffectLaunch;
    }

    private void ShakeForHit(bool crit)
    {
        StopAllCoroutines();
        StartCoroutine(Shake(crit));
    }

    private void CritEffectLaunch(bool crit)
    {
        if (crit)
        {
            critEffect.Play();
        }
    }

    IEnumerator Shake(bool crit)
    {
        float magnitude = crit ? shakeBaseMagnitude * critShakeMultiplier : shakeBaseMagnitude;
        float elapsed = 0.0f;

        Vector3 originalCamPos = cam.transform.position;
        float randomStart = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        while (elapsed < shakeDuration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / shakeDuration;

            // to reduce the shaking magnitude from full power to 0, beginning from halfway the shaking duration
            float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);

            float alpha = randomStart + shakeSpeed * percentComplete;

            // map value to [-1, 1], that smoothly change thanks to Perlin noise
            float x = SimplexNoise.Noise(alpha, 0);
            float y = SimplexNoise.Noise(0, alpha);
            //float x = Mathf.PerlinNoise(alpha, 0);
            //float y = Mathf.PerlinNoise(0, alpha);

            x *= magnitude * damper;
            y *= magnitude * damper;

            cam.transform.position = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

            yield return null;
        }

        cam.transform.position = originalCamPos;
    }

}
