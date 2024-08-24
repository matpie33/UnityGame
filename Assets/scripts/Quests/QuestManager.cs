using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class QuestManager : Observer
{
    private Dictionary<Quest, int> quests = new Dictionary<Quest, int>();
    private UIUpdater uiUpdater;

    private const int maxQuests = 3;

    private EventQueue eventQueue;

    [SerializeField]
    private GenericNpc npcJim;

    [SerializeField]
    private GenericNpc npcWolves;

    private GenericNpc pendingNpcQuestConfirmation;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
        eventQueue.SubmitEvent(new EventDTO(EventType.NPC_QUEST_AVAILABLE, npcJim));
        eventQueue.SubmitEvent(new EventDTO(EventType.NPC_QUEST_AVAILABLE, npcWolves));
    }

    private void Update()
    {
        uiUpdater = FindAnyObjectByType<UIUpdater>();
    }

    public void ReceiveQuest(Quest quest)
    {
        if (quests.Count == maxQuests || quests.ContainsKey(quest))
        {
            return;
        }
        uiUpdater.AddQuestToUI(quest, quests.Count);
        quests.Add(quest, 0);
    }

    public void OnQuestStepComplete(Quest quest)
    {
        if (!quests.ContainsKey(quest))
        {
            return;
        }
        int questStep = quests[quest] + 1;
        if (questStep == quest.questParts.Count)
        {
            uiUpdater.RemoveQuestFromUI(quest);
            quests.Remove(quest);
        }
        else
        {
            uiUpdater.ChangeDescription(quest, questStep);
            quests[quest] = questStep;
        }
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        Quest quest;
        switch (eventDTO.eventType)
        {
            case EventType.QUEST_ACCEPTED:
                GenericNpc npc = (GenericNpc)eventDTO.eventData;
                quest = npc.quest;
                ReceiveQuest(quest);
                break;
            case EventType.QUEST_STEP_COMPLETED:
                quest = (Quest)eventDTO.eventData;
                OnQuestStepComplete(quest);
                break;
            case EventType.QUEST_CONFIRMATION_NEEDED:
                pendingNpcQuestConfirmation = (GenericNpc)eventDTO.eventData;
                break;
        }
    }

    public void QuestAccepted()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_ACCEPTED, pendingNpcQuestConfirmation));
        pendingNpcQuestConfirmation = null;
    }

    public void QuestRejected()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_REJECTED, pendingNpcQuestConfirmation));
        pendingNpcQuestConfirmation = null;
    }
}
