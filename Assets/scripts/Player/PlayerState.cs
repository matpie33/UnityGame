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

    public int strength { get; private set; }

    public int agility { get; private set; }

    public int defence { get; private set; }

    public int health { get; private set; }

    public bool isAttacking { get; set; }

    public Interactable objectToInteractWith { get; set; }

    public bool isPickingObject { get; set; }

    public PlayerState()
    {
        strength = 1;
        agility = 1;
        defence = 1;
        health = 1;
    }

    public void increaseStats(StatsAddingDTO statsAddingDTO)
    {
        strength += statsAddingDTO.strengthIncrease;
        agility += statsAddingDTO.agilityIncrease;
        defence += statsAddingDTO.defenceIncrease;
        health += statsAddingDTO.healthIncrease;
    }

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
