using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : MonoBehaviour
{
    [SerializeField]
    private List<Transform> pathList;

    private NavMeshAgent navMeshAgent;

    private Animator animator;

    private bool movementStart;

    [SerializeField]
    private float lerpValue;

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

        if (ActionKeys.IsKeyPressed(KeyCode.J) && pathList.Count > 0)
        {
            movementStart = true;
            Transform pathPart = pathList[0];
            pathList.Remove(pathPart);
            navMeshAgent.SetDestination(pathPart.position);
        }
        else if (!navMeshAgent.pathPending && !navMeshAgent.hasPath)
        {
            movementStart = false;
            navMeshAgent.isStopped = true;
            animator.SetBool("Walk", false);
        }
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
}
