using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthState
{
    private int healthPercent;

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
            Debug.Log("health: " + healthPercent);
        }
    }

    public bool IsAlive()
    {
        return healthPercent > 0;
    }
}