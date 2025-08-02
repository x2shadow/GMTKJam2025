using System.Collections;
using UnityEngine;
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

        // 3. Поворот к монитору
        yield return RotateCameraTowards(lookAtMonitor);

        // 4. Диалог 10
        yield return dialogueRunner.StartDialogueCoroutine(dialogueMonitor, 0);
        playerController.SetInputBlocked(true);

        // Звук двери и шагов
        newOne.SetActive(true);

        // 5. Поворот к новичку
        yield return RotateCameraTowards(lookAtNovice);

        // 6. Диалог 11
        yield return dialogueRunner.StartDialogueCoroutine(dialogueNovice, 0);
        playerController.SetInputBlocked(true);

        // 7. Затемнение
        yield return FadeToBlack(0.1f);

        // 8. Появление надписи REPLACEMENT
        replacementText.gameObject.SetActive(true);
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
