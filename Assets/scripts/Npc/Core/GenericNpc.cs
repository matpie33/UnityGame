using System.Collections;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GenericNpc : Interactable
{
    private NpcSounds npcSounds;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private string functionToExecAfterNpcSound;
    private EventQueue eventQueue;
    public GameObject lookAtTarget { get; set; }
    public bool isDoingRetry { get; private set; }

    public bool questFinished { get; set; }

    private PrefabFactory prefabFactory;

    private GameObject questMarkGameObject;

    [field: SerializeField]
    public Quest quest { get; private set; }

    private void Awake()
    {
        npcSounds = GetComponent<NpcSounds>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        eventQueue = FindObjectOfType<EventQueue>();
        prefabFactory = FindObjectOfType<PrefabFactory>();
        animator = GetComponent<Animator>();
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        GenericNpc npcQuest;
        switch (eventDTO.eventType)
        {
            case EventType.NPC_QUEST_AVAILABLE:
                GenericNpc npc = (GenericNpc)eventDTO.eventData;
                if (npc == this)
                {
                    SpawnQuestMark();
                }
                break;
            case EventType.QUEST_ACCEPTED:
                npcQuest = (GenericNpc)eventDTO.eventData;
                if (npcQuest == this)
                {
                    QuestAccepted();
                }
                break;
            case EventType.QUEST_REJECTED:
                npcQuest = (GenericNpc)eventDTO.eventData;
                if (npcQuest == this)
                {
                    QuestRejected();
                }
                break;

            case EventType.NPC_DIED:
                GameObject npcObject = (GameObject)eventDTO.eventData;
                if (npcObject == this.gameObject)
                {
                    animator.Play("Base Layer.Idle");
                    animator.SetBool("Walk", false);
                    npcSounds.ResetSoundsCounter();
                    canBeInteracted = true;
                    isDoingRetry = true;
                }
                break;
        }
    }

    private void SpawnQuestMark()
    {
        Transform thisTransform = this.gameObject.transform;
        Vector3 position =
            thisTransform.position + GetComponent<Collider>().bounds.size.y * Vector3.up;
        Quaternion rotation = thisTransform.rotation;
        questMarkGameObject = prefabFactory.GetPrefab(
            TypeOfPrefab.QUEST_MARKER,
            position,
            rotation
        );
    }

    public void SpawnQuestMark(Material material)
    {
        SpawnQuestMark();
        for (int i = 0; i < questMarkGameObject.transform.childCount; i++)
        {
            Transform child = questMarkGameObject.transform.GetChild(i);
            child.GetComponent<Renderer>().material = material;
        }
    }

    public override void Interact(Object data)
    {
        if (questFinished)
        {
            npcSounds.PlayNextMessage();
            questMarkGameObject.SetActive(false);
            SetLookAtTarget((GameObject)data);
            eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_STEP_COMPLETED, quest));
        }
        else if (!isDoingRetry)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("Walk", false);
            float clipLength = npcSounds.PlayNextMessage();
            functionToExecAfterNpcSound = nameof(SendEvent);
            Invoke(functionToExecAfterNpcSound, clipLength);
            eventQueue.SubmitEvent(new EventDTO(EventType.PLAYER_TALKING_TO_NPC, null));
            SetLookAtTarget((GameObject)data);
        }
        else if (isDoingRetry)
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_RETRY, gameObject));
        }
    }

    private void Update()
    {
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

    public void SetLookAtTarget(GameObject target)
    {
        lookAtTarget = target;
    }

    private void SendEvent()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_CONFIRMATION_NEEDED, this));
        Cursor.lockState = CursorLockMode.None;
        functionToExecAfterNpcSound = null;
    }

    private void QuestAccepted()
    {
        questMarkGameObject.SetActive(false);
        lookAtTarget = null;

        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_STEP_COMPLETED, quest));
    }

    private void QuestRejected()
    {
        lookAtTarget = null;
        canBeInteracted = true;
        npcSounds.ResetSoundsCounter();
    }
}
