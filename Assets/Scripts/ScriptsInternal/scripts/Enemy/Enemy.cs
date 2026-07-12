using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float minimumDistanceToChase;

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

    private AnimalStateMachine animalStateMachine;
    private bool isStunned;
    private float offsetForStoppingDistance = .5f;
    private bool isDead;
    private bool isInStunCooldown;
    private int stunCooldownSeconds = 15;
    private float stunTimer;
    private bool isDoingAttackAnimation;

    private void Start()
    {
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        objectsWithHealth = FindAnyObjectByType<GameManager>().objectsWithHealth;
        characterController = FindAnyObjectByType<CharacterController>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        animalStateMachine = GetComponent<AnimalStateMachine>();
    }

    public bool GetIsAttacking()
    {
        if (isAttacking)
        {
            isAttacking = false;
            Vector3 directionToPlayer = (characterController.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, directionToPlayer);
            bool isInFront = dot > 0.95f;
            return IsPlayerInAttackRange() && isInFront;
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
        isDoingAttackAnimation = false;
    }

    public void AttackAnimationStart ()
    {
        isDoingAttackAnimation = true;
    }


    private void Update()
    {
        stunTimer += Time.deltaTime;
        if (isStunned || isDead)
        {
            return;
        }

        if (!isDoingAttackAnimation)
        {
            transform.LookAt(characterController.transform.position);
        }

        float localMinDistance = Mathf.Infinity;
        Vector3 closestObject = Vector3.zero;
        
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
            if (distance < localMinDistance && distance < minimumDistanceToChase)
            {
                    
                closestObject = objectWithHealth.transform.position;
                attackedPerson = objectWithHealth;
            }
        }
        

        if (attackedPerson != null)
        {
            navMeshAgent.destination = this.attackedPerson.transform.position;

            if (IsPlayerInAttackRange())
            {

                navMeshAgent.isStopped = true;
                if (animalStateMachine.currentState == animalStateMachine.RunState)
                {
                    navMeshAgent.stoppingDistance += offsetForStoppingDistance;
                }
                animalStateMachine.ChangeState(animalStateMachine.BiteState);
            }
            else if (animalStateMachine.currentState != animalStateMachine.RunState && !isDoingAttackAnimation)
            {
                navMeshAgent.isStopped = false;
                if (animalStateMachine.currentState == animalStateMachine.BiteState)
                {
                    navMeshAgent.stoppingDistance -= offsetForStoppingDistance;
                }
                animalStateMachine.ChangeState(animalStateMachine.RunState);
            }
        }
    }

    private bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, navMeshAgent.destination) <= navMeshAgent.stoppingDistance;
    }

    public void Stun(float stunTime)
    {
        
        if (isInStunCooldown)
        {
            if (stunTimer >= stunCooldownSeconds)
            {
                isInStunCooldown = false;
            } else
            {
                return;
            }
        }
        
        isInStunCooldown = true;
        stunTimer = 0;
        animalStateMachine.ChangeState(animalStateMachine.StunState);
        isStunned = true;
        CancelInvoke(nameof(CancelStun));
        Invoke(nameof(CancelStun), stunTime);
    }

    private void CancelStun()
    {
        isStunned = false;
    }

    public void Reset()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        attackedPerson = null;
    }

    internal void ClearPath()
    {
        navMeshAgent.isStopped = true;
        animalStateMachine.ChangeState(animalStateMachine.IdleState);
    }

    internal void Die()
    {
        isDead = true;
        
        Destroy(this.GetComponentInChildren<Collider>());
        Destroy(this.GetComponentInChildren<Rigidbody>());
        Destroy(this.GetComponentInChildren<NavMeshAgent>());
        this.animalStateMachine.animalAnimationsManager.setAnimationToDeath();

    }


}
