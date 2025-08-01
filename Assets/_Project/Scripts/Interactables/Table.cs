using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    [Tooltip("UI-подсказка 'Взять модуль'")]
    public GameObject promptUI;

    [Tooltip("Слот в камере, в который нужно поместить модуль")]
    public Transform tableSlot;
    public Transform playerSlot;


    [Tooltip("Сам объект модуля, который лежит рядом и должен быть взят")]
    public Transform chip;

    private bool hasModule = false;

    private void OnTriggerEnter(Collider other) { if (promptUI != null && !hasModule) promptUI.SetActive(true); }
    private void OnTriggerExit(Collider other)  { if (promptUI != null) promptUI.SetActive(false); }

    public void Interact(PlayerController player)
    {
        var module = player.HeldModule;

        if (module?.CurrentState == ModuleController.State.InHand)
        {
            module?.PlaceOnTable();

            hasModule = true;
            promptUI?.SetActive(false);
            Debug.Log("Модуль на столе!");

            // Прикрепляем к слоту
            module.transform.SetParent(tableSlot, worldPositionStays: true);
            module.transform.localPosition = Vector3.zero;
            module.transform.localRotation = Quaternion.identity;
        }
        else if (module?.CurrentState == ModuleController.State.ChipTaken)
        {
            module?.InsertChip();

            hasModule = true;
            promptUI?.SetActive(false);
            Debug.Log("Чип вставлен!");

            // Прикрепляем к слоту
            chip.SetParent(module.slot, worldPositionStays: true);
            chip.localPosition = Vector3.zero;
            chip.localRotation = Quaternion.identity;
        }
        else if (module?.CurrentState == ModuleController.State.ChipInserted)
        {
            module?.CompleteWiring();

            hasModule = true;
            promptUI?.SetActive(false);
            Debug.Log("Модуль готов!");

            // Прикрепляем к слоту
            module.transform.SetParent(playerSlot, worldPositionStays: true);
            module.transform.localPosition = Vector3.zero;
            module.transform.localRotation = Quaternion.identity;
        }
    }
}
