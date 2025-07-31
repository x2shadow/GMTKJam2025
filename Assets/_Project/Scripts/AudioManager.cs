using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Настройки фоновой музыки")]
    public AudioSource bgMusic;
    public AudioClip placebo;
    public AudioClip ambient;
    public float fadeDuration = 3f;

    private void Awake()
    {
        // Реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // не уничтожается при смене сцен
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutEffect());
    }

    private IEnumerator FadeOutEffect()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            bgMusic.volume = 0.1f - Mathf.Clamp01(timer / fadeDuration) / 10f;
            yield return null;
        }
        bgMusic.volume = 0f;
    }

    public void PlayPlacebo()
    {
        bgMusic.volume = 0.1f;
        bgMusic.clip = placebo;
        bgMusic.Play();
    }

    public void PlayAmbient()
    {
        bgMusic.volume = 0.1f;
        bgMusic.clip = ambient;
        bgMusic.Play();
    }
}
