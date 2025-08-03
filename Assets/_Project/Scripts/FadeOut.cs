using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{
    CanvasGroup canvasGroup;

    [Tooltip("Скорость затухания (секунд)")]
    public float fadeDuration = 1f;

        private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Start()
    {
        StartFadeOut();
    }
    
    public void StartFadeOut()
    {
        canvasGroup.alpha = 0f;
        StartCoroutine(FadeOutEffect());
    }

    void OnTriggerEnter(Collider other)
    {
        //StartCoroutine(Part2Music());
        StartCoroutine(FadeOutEffect());   
    }

    private IEnumerator Part2Music()
    {
        AudioManager.Instance.FadeOut();
        yield return new WaitForSeconds(3f); // Audio FadeOut duration
        StartCoroutine(FadeOutEffect());
        //AudioManager.Instance.PlayAmbient2();
    }


    private IEnumerator FadeOutEffect()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }
}
