using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class PlayerState
{
    public int numberOfMedipacks { get; private set; }

    public bool isAttacking { get; set; }

    public Interactable objectToInteractWith { get; set; }

    public bool isPickingObject { get; set; }

    public void increaseMedipacksAmount()
    {
        numberOfMedipacks++;
    }

    public void decreaseMedipacksAmount()
    {
        numberOfMedipacks--;
    }

    internal bool HasMedipacks()
    {
        return numberOfMedipacks > 0;
    }
}
