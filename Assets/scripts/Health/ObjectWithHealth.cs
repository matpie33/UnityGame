using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.PlayerLoop.PreUpdate;

public class ObjectWithHealth : Observer
{
    public HealthState healthState { get; private set; }

    [field: SerializeField]
    public TypeOfObjectWithHealth type { get; private set; }

    [SerializeField]
    private int maxHealth;

    [field: SerializeField]
    public bool skipHealthBar { get; private set; }

    [field: SerializeField]
    public Stats stats { get; private set; }

    private void Awake()
    {
        healthState = new HealthState(maxHealth);
    }

    public void DecreaseHealth(int byValue)
    {
        healthState.DecreaseHealth(byValue);
    }

    public bool IsAlive()
    {
        return healthState.IsAlive();
    }

    internal void ResetHealth()
    {
        healthState = new HealthState(maxHealth);
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.RESET_HEALTH:
                ObjectWithHealth objectWithHealth = (ObjectWithHealth)eventDTO.eventData;
                objectWithHealth.ResetHealth();
                break;
        }
    }
}
