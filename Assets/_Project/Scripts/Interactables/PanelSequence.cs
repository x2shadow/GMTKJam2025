using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PanelSequence : MonoBehaviour, IInteractable
{
    [Header("Ссылки")]
    public GameObject promptUI;
    public Transform playerTransform;
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

        // 2. Диалог 9
        yield return dialogueRunner.StartDialogueCoroutine(dialogueNote, 0);
        playerController.SetInputBlocked(true);

        // 3. Поворот к монитору
        yield return RotateTowards(lookAtMonitor);

        // 4. Диалог 10
        yield return dialogueRunner.StartDialogueCoroutine(dialogueMonitor, 0);
        playerController.SetInputBlocked(true);

        // 5. Поворот к новичку
        yield return RotateTowards(lookAtNovice);

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
        Quaternion startRot = playerTransform.rotation;
        Vector3 dir = (target.position - playerTransform.position).normalized;
        Quaternion endRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        float t = 0;

        while (t < rotateDuration)
        {
            t += Time.deltaTime;
            float lerpT = Mathf.Clamp01(t / rotateDuration);
            playerTransform.rotation = Quaternion.Slerp(startRot, endRot, lerpT);
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
