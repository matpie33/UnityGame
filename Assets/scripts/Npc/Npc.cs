using System;
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
    private GameObject wolvesGroupObject;

    [SerializeField]
    private GameObject questMarkPrefab;

    private GameObject questMarkGameObject;

    private GameObject lookAtTarget;

    private EventQueue eventQueue;

    [SerializeField]
    private Quest escortJimQuest;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        eventQueue = FindObjectOfType<EventQueue>();
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
            && !wolvesGroupObject.activeSelf
        )
        {
            wolvesGroupObject.SetActive(true);
            eventQueue.SubmitEvent(new EventDTO(EventType.NPC_ATTACKED, null));
        }

        if (StoppedMoving())
        {
            movementStart = false;
            navMeshAgent.isStopped = true;
            animator.SetBool("Walk", false);
            pathProgress++;
        }

        if (lookAtTarget != null)
        {
            gameObject.transform.rotation = Quaternion.Lerp(
                gameObject.transform.rotation,
                Quaternion.LookRotation(
                    lookAtTarget.transform.position - gameObject.transform.position
                ),
                0.1f
            );
            ;
        }
    }

    public void SetLookAtTarget(GameObject target)
    {
        lookAtTarget = target;
    }

    private bool StoppedMoving()
    {
        return movementStart && !navMeshAgent.pathPending && !navMeshAgent.hasPath;
    }

    public void QuestAccepted()
    {
        questMarkGameObject.SetActive(false);
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_RECEIVED, escortJimQuest));
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_CONFIRMATION_DONE, null));
        lookAtTarget = null;

        Invoke("ScheduleMove", 1.5f);
    }

    private void ScheduleMove()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.NPC_MOVE, null));
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_STEP_COMPLETED, escortJimQuest));
    }

    public void QuestRejected()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_CONFIRMATION_DONE, null));
        GetComponent<NpcInteractions>().enabled = true;
        lookAtTarget = null;
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
            case EventType.NPC_QUEST_AVAILABLE:
                Npc npc = (Npc)eventDTO.eventData;
                if (npc == this)
                {
                    questMarkGameObject = Instantiate(questMarkPrefab, gameObject.transform);
                }
                break;
        }
    }

    private void MoveToNextPoint()
    {
        if (pathList.Count == 0)
        {
            return;
        }
        movementStart = true;
        Transform destination1 = pathList[0];
        pathList.Remove(destination1);
        navMeshAgent.SetDestination(destination1.position);
    }

    internal void QuestInProgress()
    {
        GetComponent<NpcInteractions>().enabled = false;
    }
}
