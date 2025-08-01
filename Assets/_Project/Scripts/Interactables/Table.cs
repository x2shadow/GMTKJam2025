using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    [Tooltip("UI-подсказка 'Взять модуль'")]
    public GameObject promptUI;

    [Tooltip("Слот в камере, в который нужно поместить модуль")]
    public Transform slot;

    private bool hasModule = false;

    private void OnTriggerEnter(Collider other) { if (promptUI != null && !hasModule) promptUI.SetActive(true); }
    private void OnTriggerExit(Collider other)  { if (promptUI != null) promptUI.SetActive(false); }

    public void Interact(PlayerController player)
    {
        var module = player.HeldModule;
        module?.PlaceOnTable();

        hasModule = true;
        promptUI?.SetActive(false);
        Debug.Log("Модуль на столе!");

        // Прикрепляем к слоту
        module.transform.SetParent(slot, worldPositionStays: true);
        module.transform.localPosition = Vector3.zero;
        module.transform.localRotation = Quaternion.identity;
    }
}
