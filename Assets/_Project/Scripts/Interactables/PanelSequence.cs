using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelSequence : MonoBehaviour, IInteractable
{
    [Header("Ссылки")]
    public GameObject promptUI;
    public GameObject newOne;
    public Transform playerTransform;
    public Transform cameraTransform;
    public PlayerController playerController;
    public Transform targetPosition; // Куда подъезжает игрок
    public Transform lookAtNote;
    public Transform lookAtMonitor;
    public Transform lookAtNovice;
    public CanvasGroup fadeCanvasGroup; // Черный экран (UI), alpha = 0..1
    public Text replacementText; // Надпись REPLACEMENT

    [Header("Диалоги")]
    public DialogueRunner dialogueRunner;
    public DialogueScript dialogueNote;
    public DialogueScript dialogueMonitor;
    public DialogueScript dialogueNovice;

    public float moveDuration = 1.5f;
    public float rotateDuration = 1f;

    [Header("Last Scene Sounds")]
    [Tooltip("Тяжело открывается дверь")]
    public AudioClip doorOpenClip;
    [Tooltip("Тяжело закрывается дверь")]
    public AudioClip doorCloseClip;
    [Tooltip("Шаги новичка")]
    public AudioClip NewOneEnterClip;

    public AudioSource audioSource;

    private bool wasUsed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (wasUsed) return;
        if (promptUI != null) promptUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    public void Interact(PlayerController player)
    {
        if (wasUsed) return;
        wasUsed = true;
        if (promptUI != null) promptUI.SetActive(false);
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        // 0. Плавное перемещение игрока
        playerController.SetInputBlocked(true);
        playerController.dialogueTarget = null;
        Vector3 startPos = playerTransform.position;
        Quaternion startRot = playerTransform.rotation;
        Vector3 endPos = targetPosition.position;

        float t = 0;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float lerpT = Mathf.Clamp01(t / moveDuration);
            playerTransform.position = Vector3.Lerp(startPos, endPos, lerpT);
            yield return null;
        }

        // 1. Поворот к записке
        yield return RotateTowards(lookAtNote);
        yield return RotateCameraTowards(lookAtNote);

        // 2. Диалог 9
        yield return dialogueRunner.StartDialogueCoroutine(dialogueNote, 0);
        playerController.SetInputBlocked(true);

        // Пауза
        yield return new WaitForSeconds(0.5f);

        // 3. Поворот к монитору
        yield return RotateCameraTowards(lookAtMonitor);
        yield return new WaitForSeconds(1.5f);

        // 4. Диалог 10
        yield return dialogueRunner.StartDialogueCoroutine(dialogueMonitor, 0);
        playerController.SetInputBlocked(true);

        // Пауза
        yield return new WaitForSeconds(0.5f);

        // Входит новичок
        newOne.SetActive(true);
        // Звук открытия двери
        audioSource.PlayOneShot(doorOpenClip);
        yield return new WaitForSeconds(doorCloseClip.length);
        // Пауза
        yield return new WaitForSeconds(0.5f);
        // Тяжело закрывается дверь
        audioSource.PlayOneShot(doorCloseClip);
        yield return new WaitForSeconds(doorCloseClip.length);
        // Звук шагов
        audioSource.PlayOneShot(NewOneEnterClip, 0.7f);
        yield return new WaitForSeconds(NewOneEnterClip.length);

        // 5. Поворот к новичку
        yield return RotateCameraTowards(lookAtNovice);
        yield return new WaitForSeconds(1.5f);

        // 6. Диалог 11
        yield return dialogueRunner.StartDialogueCoroutine(dialogueNovice, 0);
        playerController.SetInputBlocked(true);

        // 7. Затемнение
        yield return FadeToBlack(0.1f);

        // 8. Появление надписи REPLACEMENT
        //replacementText.gameObject.SetActive(true);
        SceneManager.LoadScene("Credits");
    }

    private IEnumerator RotateTowards(Transform target)
    {
        // Поворот тела игрока по горизонтали
        Vector3 dir = (target.position - playerTransform.position).normalized;
        dir.y = 0; // только горизонтальная составляющая
        Quaternion startRot = playerTransform.rotation;
        Quaternion endRot = Quaternion.LookRotation(dir);

        float t = 0f;
        while (t < rotateDuration)
        {
            t += Time.deltaTime;
            float lerpT = Mathf.Clamp01(t / rotateDuration);
            playerTransform.rotation = Quaternion.Slerp(startRot, endRot, lerpT);
            yield return null;
        }

        // Поворот камеры по вертикали (вверх/вниз)
        yield return StartCoroutine(RotateCameraTowards(target));
    }

    private IEnumerator RotateCameraTowards(Transform target)
    {
        Vector3 dirToTarget = (target.position - cameraTransform.position).normalized;
        Quaternion startRot = cameraTransform.rotation;

        // Создаём временный объект, чтобы вычислить правильный "смотрящий" поворот
        GameObject dummy = new GameObject("LookTargetDummy");
        dummy.transform.position = cameraTransform.position;
        dummy.transform.LookAt(target); // Поворачиваем его точно как надо
        Quaternion endRot = dummy.transform.rotation;
        Destroy(dummy); // Больше не нужен

        float t = 0f;
        while (t < rotateDuration)
        {
            t += Time.deltaTime;
            float lerpT = Mathf.Clamp01(t / rotateDuration);
            cameraTransform.rotation = Quaternion.Slerp(startRot, endRot, lerpT);
            yield return null;
        }
    }

    private IEnumerator FadeToBlack(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }
    }
}
