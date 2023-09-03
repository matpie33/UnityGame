using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private EnemyState enemyState = new EnemyState();

    public NavMeshAgent navMeshAgent { get; private set; }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void DecreaseHealth(int byValue)
    {
        enemyState.DecreaseHealth(byValue);
    }
}
