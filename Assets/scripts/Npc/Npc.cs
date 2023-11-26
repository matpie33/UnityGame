using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Npc : Interactable
{
    private List<Transform> pathList = new List<Transform>();

    [SerializeField]
    private GameObject pathParent;

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

    private NpcSounds npcSounds;

    [SerializeField]
    private GameObject areaLookTarget1;

    [SerializeField]
    private GameObject areaLookTarget2;

    [SerializeField]
    private Quest escortJimQuest;

    private string functionToExecAfterNpcSound;

    [SerializeField]
    private float desiredAngle;

    private bool isDoingRetry;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        eventQueue = FindObjectOfType<EventQueue>();
        npcSounds = GetComponent<NpcSounds>();
        ResetPathList();
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
            npcSounds.PlayUnderAttack();
            animator.CrossFade("Base Layer.Terrified", 0.1f);
        }

        if (StoppedMoving())
        {
            movementStart = false;
            navMeshAgent.isStopped = true;
            animator.SetBool("Walk", false);
            pathProgress++;
            PathReached();
        }

        if (lookAtTarget != null)
        {
            LookAtTarget();
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.SKIP_NPC_AUDIO))
        {
            if (functionToExecAfterNpcSound != null)
            {
                CancelInvoke(functionToExecAfterNpcSound);
                Invoke(functionToExecAfterNpcSound, 0);
                functionToExecAfterNpcSound = null;
            }
            npcSounds.StopCurrentClip();
        }
    }

    private void LookAtTarget()
    {
        gameObject.transform.rotation = Quaternion.Lerp(
            gameObject.transform.rotation,
            Quaternion.LookRotation(
                lookAtTarget.transform.position - gameObject.transform.position
            ),
            0.1f
        );
    }

    private void PathReached()
    {
        if (pathProgress == 3)
        {
            StartCoroutine(LookAround());
        }
    }

    private IEnumerator LookAround()
    {
        npcSounds.PlayAdmireViews();
        yield return new WaitForSeconds(1f);
        lookAtTarget = areaLookTarget1;
        yield return new WaitForSeconds(3f);
        lookAtTarget = areaLookTarget2;
        yield return new WaitForSeconds(4f);
        lookAtTarget = null;
        StartMoving();
    }

    public override void Interact(Object data)
    {
        if (!isDoingRetry)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("Walk", false);
            float clipLength = npcSounds.PlayHelloMessage();
            functionToExecAfterNpcSound = nameof(SendEvent);
            Invoke(functionToExecAfterNpcSound, clipLength);
            eventQueue.SubmitEvent(new EventDTO(EventType.PLAYER_TALKING_TO_NPC, null));
            SetLookAtTarget((GameObject)data);
        }
        else
        {
            MoveToNextPoint();
        }
    }

    private void SendEvent()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_CONFIRMATION_NEEDED, null));
        Cursor.lockState = CursorLockMode.None;
        functionToExecAfterNpcSound = null;
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

        Invoke(nameof(ScheduleMove), 0.5f);
    }

    private void ScheduleMove()
    {
        MoveToNextPoint();
        npcSounds.PlayLetsMove();
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_STEP_COMPLETED, escortJimQuest));
    }

    public void QuestRejected()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_CONFIRMATION_DONE, null));
        lookAtTarget = null;
        canBeInteracted = true;
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
            case EventType.NPC_QUEST_AVAILABLE:
                Npc npc = (Npc)eventDTO.eventData;
                if (npc == this)
                {
                    questMarkGameObject = Instantiate(questMarkPrefab, this.gameObject.transform);
                }
                break;
            case EventType.OBJECT_DESTROYED:
                GameObject gameObject = (GameObject)eventDTO.eventData;
                if (gameObject == wolvesGroupObject)
                {
                    Invoke(nameof(StartMoving), 1f);
                    animator.CrossFade("Base Layer.Idle", 0.1f);
                    npcSounds.PlayWeReSafe();
                }
                break;
            case EventType.NPC_DIED:
                GameObject npcObject = (GameObject)eventDTO.eventData;
                if (npcObject == this.gameObject)
                {
                    wolvesGroupObject.SetActive(false);
                    ResetPathList();
                    ObjectWithHealth npcObjectWithHealth = GetComponent<ObjectWithHealth>();
                    animator.Play("Base Layer.Idle");
                    ResetHealthForNpcAndEnemies(npcObjectWithHealth);
                    canBeInteracted = true;
                    pathProgress = 1;
                    isDoingRetry = true;
                }
                break;
        }
    }

    private void ResetHealthForNpcAndEnemies(ObjectWithHealth objectWithHealth)
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.RESET_HEALTH, objectWithHealth));
        for (int i = 0; i < wolvesGroupObject.transform.childCount; i++)
        {
            Transform child = wolvesGroupObject.transform.GetChild(i);
            child.gameObject.SetActive(true);
            child.GetComponent<Enemy>().Reset();
            ObjectWithHealth enemyObject = child.GetComponent<ObjectWithHealth>();
            eventQueue.SubmitEvent(new EventDTO(EventType.RESET_HEALTH, enemyObject));
        }
    }

    private void StartMoving()
    {
        navMeshAgent.isStopped = false;
        animator.CrossFade("Base Layer.Walk", 0.1f);
        animator.SetBool("Walk", true);
        MoveToNextPoint();
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

    internal void QuestInProgress() { }
}
