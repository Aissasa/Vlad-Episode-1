using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{

    Animator animator;
    bool isFading;

    string fadeInTrigger = "fadingIn";
    string fadeOutTrigger = "fadingOut";

    void Start()
    {
        animator = GetComponent<Animator>();
        isFading = false;
    }

    public void AnimationComplete()
    {
        isFading = false;
    }

    public IEnumerator FadeInCoroutine()
    {
        isFading = true;
        animator.SetTrigger(fadeInTrigger);

        while (isFading)
        {
            yield return null;
        }
    }

    public IEnumerator FadeOutCoroutine()
    {
        isFading = true;
        animator.SetTrigger(fadeOutTrigger);

        while (isFading)
        {
            yield return null;
        }
    }


}
