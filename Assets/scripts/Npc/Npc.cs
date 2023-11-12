using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : Observer
{
    [SerializeField]
    private List<Transform> pathList;

    private int pathProgress = 1;

    private NavMeshAgent navMeshAgent;

    private Animator animator;

    private bool movementStart;

    [SerializeField]
    private float lerpValue;

    [SerializeField]
    private GameObject wolvesParentObject;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (movementStart)
        {
            RotateThenMove();
        }
        if (
            movementStart
            && pathProgress == 1
            && navMeshAgent.remainingDistance < 1f
            && !wolvesParentObject.activeSelf
        )
        {
            wolvesParentObject.SetActive(true);
        }

        if (StoppedMoving())
        {
            movementStart = false;
            navMeshAgent.isStopped = true;
            animator.SetBool("Walk", false);
            pathProgress++;
        }
    }

    private bool StoppedMoving()
    {
        return movementStart && !navMeshAgent.pathPending && !navMeshAgent.hasPath;
    }

    private void RotateThenMove()
    {
        Quaternion destinationRotation = Quaternion.LookRotation(
            navMeshAgent.destination - transform.position
        );
        float angle = Quaternion.Angle(transform.rotation, destinationRotation);
        if (angle < 70)
        {
            navMeshAgent.isStopped = false;
            animator.SetBool("Walk", true);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                destinationRotation,
                lerpValue
            );
        }
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.NPC_MOVE:
                MoveToNextPoint();
                break;
        }
    }

    private void MoveToNextPoint()
    {
        movementStart = true;
        Transform destination1 = pathList[0];
        pathList.Remove(destination1);
        navMeshAgent.SetDestination(destination1.position);
    }
}
