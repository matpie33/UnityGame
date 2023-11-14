using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Stats
{
    [field: SerializeField]
    public int strength { get; set; }

    [field: SerializeField]
    public int agility { get; set; }

    [field: SerializeField]
    public int defence { get; set; }

    [field: SerializeField]
    public int health { get; set; }

    internal void Increase(StatsAddingDTO statsAddingDTO)
    {
        strength += statsAddingDTO.strengthIncrease;
        agility += statsAddingDTO.agilityIncrease;
        defence += statsAddingDTO.defenceIncrease;
        health += statsAddingDTO.healthIncrease;
    }
}
