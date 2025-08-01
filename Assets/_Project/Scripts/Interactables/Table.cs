using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IInteractable
{
    [Tooltip("UI-подсказка 'Взять модуль'")]
    public GameObject promptUI;
    //public Text promptText;

    [Tooltip("Слот в камере, в который нужно поместить модуль")]
    public Transform tableSlot;
    public Transform playerSlot;


    [Tooltip("Сам объект модуля, который лежит рядом и должен быть взят")]
    public Transform chip;

    public ModuleGive moduleGive;

    private bool placedOnTable = false;

    private void OnTriggerEnter(Collider other)
    {
        if (promptUI == null) return;
        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        var module = player.HeldModule;
        if (module == null) return;             // Нет модуля — не показываем
        if (module.CurrentState == ModuleController.State.InHand && !placedOnTable)
        {
            ShowPrompt("Положить модуль");
        }
        else if (module.CurrentState == ModuleController.State.ChipTaken)
        {
            ShowPrompt("Вставить чип");
        }
        else if (module.CurrentState == ModuleController.State.ChipInserted)
        {
            ShowPrompt("Паять проводку");
        }
    }

    private void ShowPrompt(string text)
    {
        //promptText.text = text;
        promptUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other) { if (promptUI != null) promptUI.SetActive(false); }

    public void Interact(PlayerController player)
    {
        var module = player.HeldModule;

        if (module?.CurrentState == ModuleController.State.InHand && !placedOnTable)
        {
            module?.PlaceOnTable();

            placedOnTable = true;
            promptUI?.SetActive(false);
            Debug.Log("Модуль на столе!");

            // Прикрепляем к слоту
            var desiredSize = Vector3.one * 0.2f;
            module.transform.SetParentWithWorldScale(tableSlot, desiredSize, worldPositionStays: true);
            module.transform.localPosition = Vector3.zero;
            module.transform.localRotation = Quaternion.identity;
            //module.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        else if (module?.CurrentState == ModuleController.State.ChipTaken)
        {
            module?.InsertChip();

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

            placedOnTable = true;
            promptUI?.SetActive(false);
            Debug.Log("Модуль готов!");

            // Прикрепляем к слоту
            module.transform.SetParent(playerSlot, worldPositionStays: true);
            module.transform.localPosition = Vector3.zero;
            module.transform.localRotation = Quaternion.identity;

            // Открываем окно сдачи модулей
            moduleGive.Reset();
        }
    }
    
    public void Reset()
    {
        placedOnTable = false;   
    }
}
