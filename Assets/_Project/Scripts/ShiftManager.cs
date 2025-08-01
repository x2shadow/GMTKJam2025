using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftManager : MonoBehaviour
{
    public int modulesPerShift = 3;
    public int totalShifts = 4;

    [Header("Текущая смена")]
    public int currentShift = 1;
    public int modulesDelivered = 0;

    public static event Action OnModuleDelivered;
    public static event Action OnShiftCompleted;
    public static event Action OnNewShiftStarted;
    public static event Action OnLastShiftStarted;

    [Header("Ссылки для запуска диалогов")]
    public DialogueRunner dialogueRunner;
    public DialogueScript dialogue2;
    public DialogueScript dialogue3;
    public DialogueScript dialogue5;

    private void OnEnable()
    {
        OnModuleDelivered += HandleModuleDelivered;
    }
    private void OnDisable()
    {
        OnModuleDelivered -= HandleModuleDelivered;
    }

    public static void NotifyModuleDelivered()
    {
        OnModuleDelivered?.Invoke();
    }

    private void HandleModuleDelivered()
    {
        modulesDelivered++;
        if (modulesDelivered >= modulesPerShift)
        {
            OnShiftCompleted?.Invoke();
        }

        HandleStory();
    }

    public void HandleStory()
    {
        if (currentShift == 1 && modulesDelivered == 1)
        {
            dialogueRunner.StartDialogue(dialogue2, 0);
        }
        else if (currentShift == 1 && modulesDelivered == 3)
        {
            dialogueRunner.StartDialogue(dialogue3, 0);
        }
        else if (currentShift == 2 && modulesDelivered == 2)
        {
            dialogueRunner.StartDialogue(dialogue5, 0);
        }
    }

    public void CompleteShift()
    {
        if (currentShift < totalShifts)
        {
            currentShift++;
            modulesDelivered = 0;
            OnNewShiftStarted?.Invoke();
            // Тут можно показать UI «Смена X началась»
        }
        else
        {
            // Конец игры
            OnLastShiftStarted?.Invoke();
            Debug.Log("Последняя смена...");
        }
    }
}

