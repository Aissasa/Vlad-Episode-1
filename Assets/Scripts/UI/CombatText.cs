using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatText : MonoBehaviour
{

    private Vector2 direction;
    private float textSpeed;
    private float fadeDelay;

    private float startAlpha;

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    void Update()
    {
        LinearMouvement.Instance.MoveTowards(transform, direction, textSpeed);
    }

    public void SetTextAttributs(Vector2 direction, float textSpeed, float fadeDelay)
    {
        this.direction = direction;
        this.textSpeed = textSpeed * 100;
        this.fadeDelay = fadeDelay;

    }

    IEnumerator FadeOut()
    {
        Text textComponent = GetComponent<Text>();
        float startAlpha = textComponent.color.a;
        float fadeRate = 1.0f / fadeDelay;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tempColor = textComponent.color;
            textComponent.color = new Color(tempColor.r, tempColor.g, tempColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += fadeRate * Time.deltaTime;

            yield return null;

        }

        Destroy(gameObject);
    }
}
