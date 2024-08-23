using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : Observer
{
    private readonly float minimumDistanceToChase = 10;
    private float minimumDistanceToAttack = 2;
    private bool isAttacking;
    public bool isInRange { get; set; }
    public NavMeshAgent navMeshAgent { get; private set; }

    [field: SerializeField]
    public int experienceValue { get; private set; }

    private List<ObjectWithHealth> objectsWithHealth;

    public ObjectWithHealth attackedPerson { get; private set; }

    private CharacterController characterController;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    [field: SerializeField]
    public EnemyType enemyType { get; private set; }

    private WolfStateMachine wolfStateMachine;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        objectsWithHealth = FindObjectOfType<GameManager>().objectsWithHealth;
        characterController = FindObjectOfType<CharacterController>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        wolfStateMachine = GetComponent<WolfStateMachine>();
    }

    public bool GetIsAttacking()
    {
        if (isAttacking)
        {
            isAttacking = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AttackStarts()
    {
        isAttacking = true;
    }

    public void FinishedAttack()
    {
        isAttacking = false;
    }

    private void Update()
    {
        float minDistance = Mathf.Infinity;
        Vector3 closestObject = Vector3.zero;
        if (attackedPerson != null)
        {
            closestObject = attackedPerson.transform.position;
            minDistance = Vector3.Distance(
                navMeshAgent.transform.position,
                attackedPerson.transform.position
            );
        }
        else
        {
            foreach (ObjectWithHealth objectWithHealth in objectsWithHealth)
            {
                if (objectWithHealth.type.Equals(TypeOfObjectWithHealth.ENEMY))
                {
                    continue;
                }
                float distance = Vector3.Distance(
                    navMeshAgent.transform.position,
                    objectWithHealth.transform.position
                );
                if (distance < minDistance && distance < minimumDistanceToChase)
                {
                    closestObject = objectWithHealth.transform.position;
                    minDistance = distance;
                    attackedPerson = objectWithHealth;
                }
            }
        }

        ChaseAndAttack(closestObject, minDistance);
    }

    private void ChaseAndAttack(Vector3 targetPosition, float distance)
    {
        if (distance < minimumDistanceToChase)
        {
            navMeshAgent.SetDestination(targetPosition);

            if (distance < minimumDistanceToAttack)
            {
                Quaternion current = gameObject.transform.rotation;

                gameObject.transform.rotation = Quaternion.Lerp(
                    current,
                    Quaternion.LookRotation(
                        targetPosition - gameObject.transform.position,
                        Vector3.up
                    ),
                    0.1f
                );
            }
            else
            {
                wolfStateMachine.ChangeState(wolfStateMachine.wolfRunState);
            }
        }
        else
        {
            wolfStateMachine.ChangeState(wolfStateMachine.wolfIdleState);
            navMeshAgent.ResetPath();
            attackedPerson = null;
        }
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.OBJECT_HP_DECREASE:
                ObjectWithHealth objectWithHealth = (ObjectWithHealth)eventDTO.eventData;
                ObjectWithHealth thisObject = GetComponent<ObjectWithHealth>();
                if (thisObject == objectWithHealth)
                {
                    attackedPerson = characterController.GetComponent<ObjectWithHealth>();
                }
                break;
        }
    }

    public void Reset()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        attackedPerson = null;
    }
}
