using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StatsToValuesConverter
{
    private const int HEALTH_MULTIPLIER = 20;
    private const int BASE_ATTACK_SPEED = 1;
    private const float ATTACK_SPEED_MODIFIER = 0.03f;
    private const int HEALTH_DECREASE_MULTIPLIER = 5;

    public int ConvertHealthStatToHPIncrease(int healthStat)
    {
        return HEALTH_MULTIPLIER * healthStat;
    }

    public float ConvertAgilityToAttackSpeed(int agility)
    {
        return BASE_ATTACK_SPEED + agility * ATTACK_SPEED_MODIFIER;
    }

    public int ConvertDefenceToHealthDecrease(int defence, int strength)
    {
        return (int)Mathf.Max(0, (strength - defence) * HEALTH_DECREASE_MULTIPLIER);
    }
}
