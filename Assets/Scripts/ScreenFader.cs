using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{


    Animator _animator;
    [SerializeField]
    bool _is_Fading;

    void Start()
    {

        _animator = GetComponent<Animator>();
        _is_Fading = false;
    }

    void Update()
    {


    }

    public IEnumerator FadeOutCoroutine()
    {
        _is_Fading = true;
        _animator.SetTrigger("fade_out");

        while (_is_Fading)
            yield return null;
    }

    public IEnumerator FadeInCoroutine()
    {
        _is_Fading = true;
        _animator.SetTrigger("fade_in");

        while (_is_Fading)
            yield return null;
    }


    public void AnimationComplete()
    {
        _is_Fading = false;
    }
}
