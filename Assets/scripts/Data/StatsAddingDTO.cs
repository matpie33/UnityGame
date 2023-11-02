using UnityEditor;
using UnityEngine;

public class StatsAddingDTO
{
    public int statsLeft { get; set; }

    public int strengthIncrease { get; private set; }
    public int agilityIncrease { get; private set; }
    public int defenceIncrease { get; private set; }
    public int healthIncrease { get; private set; }

    public StatsAddingDTO()
    {
        statsLeft = PlayerConstants.AMOUNT_OF_STATS_TO_ADD_PER_LEVER;
    }

    public void Reset()
    {
        strengthIncrease = 0;
        agilityIncrease = 0;
        defenceIncrease = 0;
        healthIncrease = 0;
        this.statsLeft = PlayerConstants.AMOUNT_OF_STATS_TO_ADD_PER_LEVER;
    }

    public void IncreaseStrength()
    {
        strengthIncrease++;
    }

    public void IncreaseAgility()
    {
        agilityIncrease++;
    }

    public void IncreaseDefence()
    {
        defenceIncrease++;
    }

    public void IncreaseHealth()
    {
        healthIncrease++;
    }
}
