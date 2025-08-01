using UnityEngine;

public class ExitDoor : MonoBehaviour, IInteractable
{
    [Header("Ссылки")]
    public GameObject promptUI;

    [Header("Ссылки для запуска диалогов")]
    public DialogueRunner dialogueRunner;
    public DialogueScript dialogueExitDoor;

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

    // Этот метод будет вызван извне, когда игрок нажмёт E
    public void Interact(PlayerController player)
    {
        if (wasUsed) return;

        wasUsed = true;
        promptUI?.SetActive(false);
        dialogueRunner.StartDialogue(dialogueExitDoor, 0);
        Debug.Log("Диалог запущен!");
    }
}
