using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour, Observer
{
    private const int healthDecreaseValue = 30;
    private bool isPlayerAttacking;

    private List<Enemy> enemies;

    private float minDistanceToAttack = 2;

    private CharacterController characterController;

    private void Start()
    {
        enemies = FindObjectsOfType<Enemy>().ToList<Enemy>();
        characterController = FindObjectOfType<CharacterController>();
        characterController.AddObserver(this);
    }

    public void SetPlayerIsAttacking(bool isAttacking)
    {
        isPlayerAttacking = isAttacking;
    }

    void Update()
    {
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(
                enemy.navMeshAgent.transform.position,
                characterController.transform.position
            );
            if (isPlayerAttacking && distance < minDistanceToAttack)
            {
                enemy.DecreaseHealth(healthDecreaseValue);
                Debug.Log("decrease");
                isPlayerAttacking = false;
            }
        }
    }

    public void Notify(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.PLAYER_ATTACK:
                isPlayerAttacking = eventDTO.isAttacking;
                break;
        }
    }
}
