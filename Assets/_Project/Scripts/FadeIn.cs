using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    CanvasGroup canvasGroup;

    [Tooltip("Скорость затухания (секунд)")]
    public float fadeDuration = 1f;
    public float pauseDuration = 2f;

    public bool startFadeIn = true;

    public PlayerController playerController;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        if (startFadeIn) StartFadeIn();
    }

    public void StartFadeIn()
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

    public void StartFadeOut()
    {
        canvasGroup.alpha = 0f;
        StartCoroutine(FadeOutEffect());
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

    public void StartFadeOutIn()
    {
        StartCoroutine(FadeOutInRoutine(pauseDuration));
    }

    private IEnumerator FadeOutInRoutine(float pauseDuration)
    {
        playerController.SetInputBlocked(true);

        // Начинаем затемнение
        yield return StartCoroutine(FadeOutEffect());

        // Небольшая пауза, пока экран затемнён
        yield return new WaitForSeconds(pauseDuration);

        // Затем начинаем осветление
        yield return StartCoroutine(FadeInEffect());

        playerController.SetInputBlocked(false);
    }

    public void StartLastFadeOutIn()
    {
        StartCoroutine(LastFadeOutInRoutine(pauseDuration));
    }

    private IEnumerator LastFadeOutInRoutine(float pauseDuration)
    {
        playerController.SetInputBlocked(true);

        // Начинаем затемнение
        yield return StartCoroutine(FadeOutEffect());

        // Небольшая пауза, пока экран затемнён
        yield return new WaitForSeconds(pauseDuration);

        // Добавить звуки

        // Затем начинаем осветление
        yield return StartCoroutine(FadeInEffect());
        
        playerController.SetInputBlocked(false);
    }
}
