using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class PlayerState
{
    public int numberOfMedipacks { get; private set; }

    public int strength { get; private set; }

    public int agility { get; private set; }

    public int defence { get; private set; }

    public int health { get; private set; }

    public bool isAttacking { get; set; }

    public PlayerState()
    {
        strength = 1;
        agility = 1;
        defence = 1;
        health = 1;
    }

    public void increaseStrength(int byValue)
    {
        strength += byValue;
    }

    public void increaseAgility(int byValue)
    {
        agility += byValue;
    }

    public void increaseDefence(int byValue)
    {
        defence += byValue;
    }

    public void increaseHealth(int byValue)
    {
        health += byValue;
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
