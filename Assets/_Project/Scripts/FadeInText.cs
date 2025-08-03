using UnityEngine;
using TMPro;
using System.Collections;

public class TextFadeInUGUI : MonoBehaviour
{
    public TextMeshProUGUI text1; // Первый UI-текст
    public TextMeshProUGUI text2; // Второй UI-текст
    public float fadeDuration = 2f; // Длительность эффекта в секундах
    public float delayBetweenTexts = 0.5f; // Задержка между появлением текстов
    public float delayBeforeTexts = 2f;

    private void Start()
    {
        // Устанавливаем начальную прозрачность
        SetAlpha(text1, 0f);
        SetAlpha(text2, 0f);
        
        // Запускаем корутины с задержкой
        StartCoroutine(FadeInText(text1, delayBeforeTexts));
        StartCoroutine(FadeInText(text2, delayBeforeTexts + delayBetweenTexts));
    }

    private IEnumerator FadeInText(TextMeshProUGUI textElement, float initialDelay)
    {
        if (textElement == null) yield break;
        
        yield return new WaitForSeconds(initialDelay);
        
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            SetAlpha(textElement, alpha);
            yield return null;
        }
        
        SetAlpha(textElement, 1f); // Гарантируем полную непрозрачность
    }

    private void SetAlpha(TextMeshProUGUI textElement, float alpha)
    {
        Color color = textElement.color;
        color.a = alpha;
        textElement.color = color;
    }
}