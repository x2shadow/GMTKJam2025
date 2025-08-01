using UnityEngine;

public class EndShiftDoor : MonoBehaviour, IInteractable
{
    [Tooltip("UI-подсказка 'Взять модуль'")]
    public GameObject promptUI;

    public ShiftManager shiftManager;
    private bool canExit = false;

    private void OnTriggerEnter(Collider other) { if (promptUI != null && canExit) promptUI.SetActive(true); }
    private void OnTriggerExit(Collider other) { if (promptUI != null) promptUI.SetActive(false); }

    private void OnEnable()
    {
        ShiftManager.OnShiftCompleted  += () => canExit = true;
        ShiftManager.OnNewShiftStarted += () => canExit = false;
    }
    private void OnDisable()
    {
        ShiftManager.OnShiftCompleted  -= () => canExit = true;
        ShiftManager.OnNewShiftStarted += () => canExit = false;
    }

    public void Interact(PlayerController player)
    {
        if (canExit)
        {
            shiftManager.CompleteShift();
            Debug.Log("Смена закончена!");
        }
        else Debug.Log("Смена не закончена");
    }
}

