using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public int numberOfMedipacks { get; private set; }

    public int attackPower { get; private set; }

    public PlayerState()
    {
        attackPower = 10;
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
