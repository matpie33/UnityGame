using System.Collections;
using UnityEngine;

public class NpcEscort : Observer
{
    private NpcMovingOnPath npcMovingOnPath;

    [SerializeField]
    private GameObject wolvesGroupObject;

    private NpcSounds npcSounds;

    private Animator animator;

    private EventQueue eventQueue;

    private GenericNpc genericNpc;

    [SerializeField]
    private GameObject areaLookTarget1;

    [SerializeField]
    private GameObject areaLookTarget2;

    private void Start()
    {
        npcMovingOnPath = GetComponent<NpcMovingOnPath>();
        npcSounds = GetComponent<NpcSounds>();
        animator = GetComponent<Animator>();
        eventQueue = FindObjectOfType<EventQueue>();
        genericNpc = GetComponent<GenericNpc>();
    }

    private void Update()
    {
        if (
            npcMovingOnPath.IsMovingToPoint(1)
            && npcMovingOnPath.IsCloseToDestination(1)
            && !wolvesGroupObject.activeSelf
        )
        {
            {
                wolvesGroupObject.SetActive(true);
                npcSounds.PlayNextMessage();
                animator.CrossFade("Base Layer.Terrified", 0.1f);
            }
        }
    }

    private void ScheduleMove()
    {
        npcMovingOnPath.MoveToNextPoint();
        npcSounds.PlayNextMessage();
    }

    private void ResetHealthForNpcAndEnemies()
    {
        for (int i = 0; i < wolvesGroupObject.transform.childCount; i++)
        {
            Transform child = wolvesGroupObject.transform.GetChild(i);
            child.gameObject.SetActive(true);
            child.GetComponent<Enemy>().Reset();
            ObjectWithHealth enemyObject = child.GetComponent<ObjectWithHealth>();
            eventQueue.SubmitEvent(new EventDTO(EventType.RESET_HEALTH, enemyObject));
        }
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        GenericNpc npcQuest;
        GameObject npcObject;
        switch (eventDTO.eventType)
        {
            case EventType.QUEST_ACCEPTED:
                npcQuest = (GenericNpc)eventDTO.eventData;
                if (npcQuest.gameObject == this.gameObject)
                {
                    ScheduleMove();
                }
                break;
            case EventType.NPC_DIED:
                npcObject = (GameObject)eventDTO.eventData;
                if (npcObject == this.gameObject)
                {
                    wolvesGroupObject.SetActive(false);
                    ObjectWithHealth npcObjectWithHealth = GetComponent<ObjectWithHealth>();
                    eventQueue.SubmitEvent(
                        new EventDTO(EventType.RESET_HEALTH, npcObjectWithHealth)
                    );
                    ResetHealthForNpcAndEnemies();
                }
                break;
            case EventType.QUEST_RETRY:
                npcObject = (GameObject)eventDTO.eventData;
                if (npcObject == this.gameObject)
                {
                    npcMovingOnPath.MoveToNextPoint();
                    npcSounds.SetMessageIndex(2);
                }
                break;
            case EventType.NPC_REACHED_POINT:
                npcObject = (GameObject)eventDTO.eventData;
                if (npcObject == this.gameObject)
                {
                    if (npcMovingOnPath.pathProgress == 3)
                    {
                        StartCoroutine(LookAround());
                    }
                }
                break;

            case EventType.OBJECT_DESTROYED:
                GameObject gameObject = (GameObject)eventDTO.eventData;
                if (gameObject == wolvesGroupObject)
                {
                    Invoke(nameof(WolvesDead), 1f);
                    animator.CrossFade("Base Layer.Idle", 0.1f);
                    npcSounds.PlayNextMessage();
                }
                break;
        }
    }

    private void WolvesDead()
    {
        npcMovingOnPath.StartMoving();
    }

    private IEnumerator LookAround()
    {
        npcSounds.PlayNextMessage();
        yield return new WaitForSeconds(1f);
        genericNpc.lookAtTarget = areaLookTarget1;
        yield return new WaitForSeconds(3f);
        genericNpc.lookAtTarget = areaLookTarget2;
        yield return new WaitForSeconds(4f);
        genericNpc.lookAtTarget = null;
        npcMovingOnPath.StartMoving();
    }
}
