using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftManager : MonoBehaviour
{
    public int modulesPerShift = 3;
    public int totalShifts = 4;
    private int modulesDelivered = 0;
    private int currentShift = 1;

    public static event Action OnModuleDelivered;
    public static event Action OnShiftCompleted;

    private void OnEnable()
    {
        OnModuleDelivered += HandleModuleDelivered;
    }
    private void OnDisable()
    {
        OnModuleDelivered -= HandleModuleDelivered;
    }

    private void HandleModuleDelivered()
    {
        modulesDelivered++;
        if (modulesDelivered >= modulesPerShift)
        {
            OnShiftCompleted?.Invoke();
        }
    }

    public void CompleteShift()
    {
        if (currentShift < totalShifts)
        {
            currentShift++;
            modulesDelivered = 0;
            // Тут можно показать UI «Смена X началась»
        }
        else
        {
            // Конец игры
        }
    }
}

