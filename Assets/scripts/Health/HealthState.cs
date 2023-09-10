using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthState
{
    public int value { get; private set; }

    public int maxHealth { get; private set; }

    public HealthState(int maxHealth)
    {
        value = maxHealth;
        this.maxHealth = maxHealth;
    }

    public void DecreaseHealth(int value)
    {
        if (this.value > 0)
        {
            this.value -= value;
        }
    }

    public void IncreaseMaxHealth(int value)
    {
        this.maxHealth += value;
    }

    public void IncreaseHealth(int value)
    {
        this.value += value;
        if (this.value > maxHealth)
        {
            this.value = maxHealth;
        }
    }

    public bool IsAlive()
    {
        return value > 0;
    }
}
