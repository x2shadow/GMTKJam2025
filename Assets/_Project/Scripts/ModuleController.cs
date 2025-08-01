using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleController : MonoBehaviour
{
    public enum State { InHand, PlacedOnTable, ChipTaken, ChipInserted, WiringDone }
    public State CurrentState { get; set; } = State.InHand;

    [Tooltip("Слот в камере, в который нужно поместить модуль")]
    public Transform slot;

    public static event Action<ModuleController> OnModuleReadyForDelivery;

    public void PlaceOnTable()
    {
        if (CurrentState == State.InHand)
            CurrentState = State.PlacedOnTable;
    }

    public void TakeChip()
    {
        if (CurrentState == State.PlacedOnTable)
            CurrentState = State.ChipTaken;
    }

    public void InsertChip()
    {
        if (CurrentState == State.ChipTaken)
            CurrentState = State.ChipInserted;
    }

    public void CompleteWiring()
    {
        if (CurrentState == State.ChipInserted)
        {
            CurrentState = State.WiringDone;
            OnModuleReadyForDelivery?.Invoke(this);
        }
    }
}

