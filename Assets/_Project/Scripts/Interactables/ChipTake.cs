using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ChipTake : MonoBehaviour, IInteractable
{
    [Tooltip("UI-подсказка 'Взять модуль'")]
    public GameObject promptUI;

    [Tooltip("Слот в камере, в который нужно поместить модуль")]
    public Transform slot;

    [Tooltip("Сам объект модуля, который лежит рядом и должен быть взят")]
    public Transform chip;

    private bool hasModule = false;

    private void OnTriggerEnter(Collider other)
    {
        if (promptUI == null) return;
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        var module = player.HeldModule;
        if (module == null) return;             // Нет модуля — не показываем
        if (module.CurrentState == ModuleController.State.PlacedOnTable)
        {
            promptUI.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other) { if (promptUI != null) promptUI.SetActive(false); }

    public void Interact(PlayerController player)
    {
        var module = player.HeldModule;
        if (module?.CurrentState != ModuleController.State.PlacedOnTable) return;
        module?.TakeChip();

        //hasModule = true;
        promptUI?.SetActive(false);
        Debug.Log("Взят чип!");

        // Прикрепляем чип к слоту
        chip.SetParent(slot, worldPositionStays: true);
        chip.localPosition = Vector3.zero;
        chip.transform.localRotation = Quaternion.identity;
    }

    public void Reset()
    {
        hasModule = false;
    }
}
