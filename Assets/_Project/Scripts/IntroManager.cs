using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Тексты для показа")]
    [TextArea] public string[] lines;
    [Header("Ссылки")]
    public string sceneToLoad;
    public TMP_Text textField;      // TextMeshProUGUI
    public float charDelay = 0.05f; // задержка между буквами
    public float lineDelay = 1f;    // пауза между строками

    private void Start()
    {
        StartCoroutine(RunIntro());
    }

    private IEnumerator RunIntro()
    {
        // Черный экран + пустой текст
        textField.text = "";
        yield return new WaitForSeconds(0.5f);

        foreach (var line in lines)
        {
            // эффект «печатающегося» текста
            textField.text = "";
            foreach (var ch in line)
            {
                textField.text += ch;
                yield return new WaitForSeconds(charDelay);
            }
            // подождать после строки
            yield return new WaitForSeconds(lineDelay);
            // плавно стереть (по желанию — эффект удаления)
            // здесь просто подменим на следующую:
        }

        // всё показано — загружаем геймплей
        SceneManager.LoadScene(sceneToLoad);
    }
}
