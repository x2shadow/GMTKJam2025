using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    CanvasGroup canvasGroup;

    [Tooltip("Скорость затухания (секунд)")]
    public float fadeDuration = 1f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // Сначала делаем полный чёрный экран
        canvasGroup.alpha = 1f;
        // Затем плавно «вводим» сцену
        StartCoroutine(FadeInEffect());
    }

    private IEnumerator FadeInEffect()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
