using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Observer
{
    private List<Quest> quests = new List<Quest>();
    private UIUpdater uiUpdater;

    private const int maxQuests = 3;

    private void Update()
    {
        uiUpdater = FindObjectOfType<UIUpdater>();
    }

    public void ReceiveQuest(Quest quest)
    {
        if (quests.Count == maxQuests || quests.Contains(quest))
        {
            return;
        }
        uiUpdater.AddQuestToUI(quest, quests.Count);
        quests.Add(quest);
    }

    public void OnQuestComplete(Quest quest)
    {
        if (!quests.Contains(quest))
        {
            return;
        }
        uiUpdater.RemoveQuestFromUI(quest);
        quests.Remove(quest);
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
            case EventType.QUEST_COMPLETED:
                quest = (Quest)eventDTO.eventData;
                OnQuestComplete(quest);
                break;
        }
    }
}
