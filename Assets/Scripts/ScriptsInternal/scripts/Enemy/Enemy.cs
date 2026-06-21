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
    private float minDistanceToAttack;
    private float offset = .3f;

    [SerializeField]
    private bool debugMinDistanceToAttack;

    private Vector3 calculatedPositionForEnemy;

    private EnemyAttackPositionSearch enemyAttackPositionSearch;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        objectsWithHealth = FindAnyObjectByType<GameManager>().objectsWithHealth;
        characterController = FindAnyObjectByType<CharacterController>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        animalStateMachine = GetComponent<AnimalStateMachine>();
        enemyAttackPositionSearch = EnemyAttackPositionSearch.INSTANCE;
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

    private void OnDrawGizmos()
    {
        if (debugMinDistanceToAttack)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(transform.position, minDistanceToAttack);
        }
    }

    private void Update()
    {
        float localMinDistance = Mathf.Infinity;
        Vector3 closestObject = Vector3.zero;

        minDistanceToAttack =
            Vector3
                .Scale(
                    GetComponentInChildren<Collider>().bounds.extents
                        + characterController.GetComponent<Collider>().bounds.extents,
                    transform.forward
                )
                .magnitude + 0;

        if (isStunned)
        {
            return;
        }
        if (attackedPerson != null)
        {
            closestObject = attackedPerson.transform.position;
            localMinDistance = Vector3.Distance(
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
                if (distance < localMinDistance && distance < minimumDistanceToChase)
                {
                    calculatedPositionForEnemy = enemyAttackPositionSearch.GetPositionForEnemy(
                        this
                    );
                    closestObject = objectWithHealth.transform.position;
                    localMinDistance = Vector3.Distance(
                        navMeshAgent.transform.position,
                        calculatedPositionForEnemy
                    );
                    attackedPerson = objectWithHealth;
                }
            }
        }

        ChaseAndAttack(closestObject, localMinDistance);
    }

    public void Stun(float stunTime)
    {
        animalStateMachine.ChangeState(animalStateMachine.StunState);
        isStunned = true;
        CancelInvoke(nameof(CancelStun));
        Invoke(nameof(CancelStun), stunTime);
    }

    private void CancelStun()
    {
        isStunned = false;
    }

    private void ChaseAndAttack(Vector3 targetPosition, float distance)
    {
        if (distance < minimumDistanceToChase)
        {
            if (distance < minDistanceToAttack)
            {
                navMeshAgent.isStopped = true;
                Quaternion current = gameObject.transform.rotation;

                gameObject.transform.rotation = Quaternion.Lerp(
                    current,
                    Quaternion.LookRotation(
                        targetPosition - gameObject.transform.position,
                        Vector3.up
                    ),
                    0.1f
                );
                animalStateMachine.ChangeState(animalStateMachine.BiteState);
            }
            else
            {
                navMeshAgent.isStopped = false;
                Vector3 destination = enemyAttackPositionSearch.GetPositionForEnemy(this);
                if (destination.Equals(Vector3.zero))
                {
                    animalStateMachine.ChangeState(animalStateMachine.IdleState);
                }
                else
                {
                    navMeshAgent.SetDestination(destination);
                    animalStateMachine.ChangeState(animalStateMachine.RunState);
                }
            }
        }
        else
        {
            animalStateMachine.ChangeState(animalStateMachine.IdleState);
            enemyAttackPositionSearch.ClearEnemyPlaceIfWasNearPlayer(this);

            attackedPerson = null;
        }
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
}
