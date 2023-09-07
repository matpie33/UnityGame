using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthState
{
    public int healthPercent { get; private set; }

    private int maxHealth = 100;

    public HealthState()
    {
        healthPercent = maxHealth;
    }

    public void DecreaseHealth(int value)
    {
        if (healthPercent > 0)
        {
            healthPercent -= value;
        }
    }

    public bool IsAlive()
    {
        return healthPercent > 0;
    }
}
