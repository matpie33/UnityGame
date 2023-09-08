using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    public int numberOfMedipacks { get; private set; }

    public void increaseMedipacksAmount()
    {
        numberOfMedipacks++;
    }

    public void decreaseMedipacksAmount()
    {
        numberOfMedipacks--;
    }
}
