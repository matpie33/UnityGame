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
    private Npc npcJim;

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
        eventQueue.SubmitEvent(new EventDTO(EventType.NPC_QUEST_AVAILABLE, npcJim));
    }

    private void Update()
    {
        uiUpdater = FindObjectOfType<UIUpdater>();
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
            case EventType.QUEST_RECEIVED:
                quest = (Quest)eventDTO.eventData;
                ReceiveQuest(quest);
                break;
            case EventType.QUEST_STEP_COMPLETED:
                quest = (Quest)eventDTO.eventData;
                OnQuestStepComplete(quest);
                break;
        }
    }
}
