using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.PlayerLoop.PreUpdate;

public class ObjectWithHealth : MonoBehaviour
{
    public HealthState healthState { get; private set; }

    [field: SerializeField]
    public TypeOfObjectWithHealth aliveObjectType { get; private set; }

    [SerializeField]
    private int maxHealth;

    private void Start()
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
}
