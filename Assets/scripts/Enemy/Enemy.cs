using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : Observer
{
    private readonly float minimumDistanceToChase = 10;
    private Animator animator;
    private float minimumDistanceToAttack = 2;
    private bool isAttacking;
    public bool isInRange { get; set; }
    public NavMeshAgent navMeshAgent { get; private set; }

    public int experienceValue { get; private set; }

    private List<ObjectWithHealth> objectsWithHealth;

    public ObjectWithHealth attackedPerson { get; private set; }

    private CharacterController characterController;

    private void Start()
    {
        experienceValue = 400;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        objectsWithHealth = FindObjectOfType<GameManager>().objectsWithHealth;
        characterController = FindObjectOfType<CharacterController>();
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
                animator.SetBool(WolfAnimationVariables.IS_ATTACKING, true);
                animator.SetBool(WolfAnimationVariables.IS_CHASING, false);

                gameObject.transform.LookAt(targetPosition, Vector3.up);
            }
            else
            {
                animator.SetBool(WolfAnimationVariables.IS_CHASING, true);
                animator.SetBool(WolfAnimationVariables.IS_ATTACKING, false);
            }
        }
        else
        {
            animator.SetBool(WolfAnimationVariables.IS_CHASING, false);
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
}
