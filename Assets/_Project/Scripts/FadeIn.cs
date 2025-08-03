using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    CanvasGroup canvasGroup;

    [Tooltip("Скорость затухания (секунд)")]
    public float fadeInDuration = 2f;
    public float fadeoutDuration = 1f;
    public float pauseDuration = 2f;

    public bool startFadeIn = true;

    public PlayerController playerController;
    public Transform fourthShiftLookTarget;

    [Header("After Third Shift Sounds")]
    [Tooltip("Тяжело открывается дверь")]
    public AudioClip doorOpenClip;
    [Tooltip("Резкий электрический треск")]
    public AudioClip electricCrackClip;
    [Tooltip("Взрывной крик")]
    public AudioClip screamClip;
    [Tooltip("Тяжёлый мешок тащат по полу")]
    public AudioClip bagDragClip;
    [Tooltip("Тяжело закрывается дверь")]
    public AudioClip doorCloseClip;

    [Header("Надписи смен")]
    public ShiftManager shiftManager;
    public GameObject shift1;
    public GameObject shift2;
    public GameObject shift3;
    public GameObject shiftX;

    public AudioClip shiftChangeClip;
    public AudioSource audioSource;

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
        if (shiftManager.currentShift == 1)
        { 
            shift1.SetActive(true);
            audioSource.PlayOneShot(shiftChangeClip);
            yield return new WaitForSeconds(2f);
            shift1.SetActive(false);
        }

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(timer / fadeInDuration);
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
        while (timer < fadeoutDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(timer / fadeoutDuration);
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

        if (shiftManager.currentShift == 2) shift2.SetActive(true);
        if (shiftManager.currentShift == 3) shift3.SetActive(true);
        audioSource.PlayOneShot(shiftChangeClip);

        // Небольшая пауза, пока экран затемнён
        yield return new WaitForSeconds(pauseDuration / 2f);


        // Костыль, чтобы повернуть игрока
        playerController.isDialogueActive = true;
        playerController.RotateTowardsDialogueTarget();
        yield return new WaitForSeconds(pauseDuration / 2f);

        if (shiftManager.currentShift == 2) shift2.SetActive(false);
        if (shiftManager.currentShift == 3) shift3.SetActive(false);


        // Затем начинаем осветление
        yield return StartCoroutine(FadeInEffect());
        playerController.isDialogueActive = false;
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

        // Пауза

        yield return new WaitForSeconds(2f);

        // Костыль, чтобы повернуть игрока
        playerController.isDialogueActive = true;
        playerController.dialogueTarget = fourthShiftLookTarget;
        playerController.RotateTowardsDialogueTarget();

        // Тяжело открывается дверь
        audioSource.PlayOneShot(doorOpenClip);
        yield return new WaitForSeconds(doorOpenClip.length);

        // Пауза
        yield return new WaitForSeconds(1.5f);

        // Резкий, оглушительный ЭЛЕКТРИЧЕСКИЙ ТРЕСК (как короткое замыкание или разряд)
        audioSource.PlayOneShot(electricCrackClip, 0.8f);
        yield return new WaitForSeconds(electricCrackClip.length/2f);

        // Взрывной крик
        audioSource.PlayOneShot(screamClip);
        yield return new WaitForSeconds(screamClip.length);

        // Пауза
        yield return new WaitForSeconds(1.5f);

        // Завершаем первый эмбиент
        AudioManager.Instance.FadeOut();

        // Тяжёлый мешок тащат по полу
        audioSource.PlayOneShot(bagDragClip);
        yield return new WaitForSeconds(bagDragClip.length);

        // Тяжело закрывается дверь
        audioSource.PlayOneShot(doorCloseClip);
        yield return new WaitForSeconds(doorCloseClip.length);

        // Стартуем второй эмбиент
        AudioManager.Instance.PlayAmbient2();
        
        // Тишина несколько секунд
        yield return new WaitForSeconds(2f);

        // Показываем надпись
        shiftX.SetActive(true);
        audioSource.PlayOneShot(shiftChangeClip);
        yield return new WaitForSeconds(2f);
        shiftX.SetActive(false);


        // Затем начинаем осветление
        yield return StartCoroutine(FadeInEffect());

        playerController.moveSpeed = 1.5f;
        playerController.isDialogueActive = false;
        playerController.SetInputBlocked(false);
    }
}
