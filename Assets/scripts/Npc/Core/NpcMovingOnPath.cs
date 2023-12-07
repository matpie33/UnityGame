using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NpcMovingOnPath : Observer
{
    private List<Transform> pathList = new List<Transform>();

    [SerializeField]
    private GameObject pathParent;

    public int pathProgress { get; private set; } = 1;

    private NavMeshAgent navMeshAgent;

    private Animator animator;

    private bool movementStart;

    private bool isRotatingTowardsTarget;

    [SerializeField]
    private float lerpValue;

    [SerializeField]
    private float desiredAngle;

    private EventQueue eventQueue;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        eventQueue = FindObjectOfType<EventQueue>();
        animator = GetComponent<Animator>();
        ResetPathList();
    }

    public bool ReachedDestination()
    {
        return pathList.Count == 0;
    }

    public void StartMoving()
    {
        navMeshAgent.isStopped = false;
        animator.CrossFade("Base Layer.Walk", 0.1f);
        animator.SetBool("Walk", true);
        MoveToNextPoint();
    }

    public void MoveToNextPoint()
    {
        isRotatingTowardsTarget = true;
        if (pathList.Count == 0)
        {
            return;
        }
        movementStart = true;
        Transform destination1 = pathList[0];
        pathList.Remove(destination1);
        navMeshAgent.SetDestination(destination1.position);
    }

    private void ResetPathList()
    {
        Transform pathTransform = pathParent.transform;
        pathList.Clear();
        for (int i = 0; i < pathTransform.childCount; i++)
        {
            Transform child = pathTransform.GetChild(i);
            pathList.Add(child);
        }
        transform.position = pathList[0].position;
        pathList.RemoveAt(0);
    }

    private void Update()
    {
        if (isRotatingTowardsTarget)
        {
            RotateThenMove();
        }

        if (movementStart && !isRotatingTowardsTarget && StoppedMoving())
        {
            movementStart = false;
            navMeshAgent.isStopped = true;
            animator.SetBool("Walk", false);
            pathProgress++;
            eventQueue.SubmitEvent(new EventDTO(EventType.NPC_REACHED_POINT, gameObject));
        }
    }

    public bool IsMovingToPoint(int pointIndex)
    {
        return movementStart && pathProgress == pointIndex;
    }

    public bool IsCloseToDestination(int distance)
    {
        return navMeshAgent.remainingDistance <= distance;
    }

    private bool StoppedMoving()
    {
        return !isRotatingTowardsTarget && !navMeshAgent.pathPending && !navMeshAgent.hasPath;
    }

    private void RotateThenMove()
    {
        Quaternion destinationRotation = Quaternion.LookRotation(
            navMeshAgent.destination - transform.position
        );
        float angle = Quaternion.Angle(transform.rotation, destinationRotation);
        if (angle < desiredAngle)
        {
            navMeshAgent.isStopped = false;
            animator.SetBool("Walk", true);
            isRotatingTowardsTarget = false;
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
            case EventType.NPC_DIED:
                GameObject npcObject = (GameObject)eventDTO.eventData;
                if (npcObject == this.gameObject)
                {
                    ResetPathList();
                    pathProgress = 1;
                    movementStart = false;
                    navMeshAgent.isStopped = true;
                }
                break;
        }
    }
}
