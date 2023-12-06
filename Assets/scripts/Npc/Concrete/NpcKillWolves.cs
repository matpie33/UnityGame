using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class NpcKillWolves : Observer
{
    private NpcSounds npcSounds;

    [SerializeField]
    private EnemyType enemyToKill;

    [SerializeField]
    private int killsNeeded;

    private int killCounter;

    private UIUpdater uiUpdater;

    private string REMAINING_KILLS_TEXT = "Remaining wolves to kill: {0}";

    private Quest quest;

    private GenericNpc genericNpc;

    [SerializeField]
    private Material yellowColorMaterial;

    private EventQueue eventQueue;

    private void Start()
    {
        npcSounds = GetComponent<NpcSounds>();
        killCounter = killsNeeded;
        uiUpdater = FindObjectOfType<UIUpdater>();
        quest = GetComponent<GenericNpc>().quest;
        UpdateRemainingKills();
        genericNpc = GetComponent<GenericNpc>();
        eventQueue = FindObjectOfType<EventQueue>();
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        GenericNpc npcQuest;
        switch (eventDTO.eventType)
        {
            case EventType.QUEST_ACCEPTED:
                npcQuest = (GenericNpc)eventDTO.eventData;
                if (npcQuest.gameObject == this.gameObject)
                {
                    npcSounds.PlayNextMessage();
                    killCounter = killsNeeded;
                    Invoke(nameof(UpdateRemainingKills), 0.05f);
                }
                break;
            case EventType.ENEMY_KILLED:
                GameObject data = (GameObject)eventDTO.eventData;
                EnemyType enemyType = data.GetComponent<Enemy>().enemyType;
                if (enemyType.Equals(this.enemyToKill))
                {
                    killCounter--;
                    UpdateRemainingKills();
                    if (killCounter == 0)
                    {
                        genericNpc.SpawnQuestMark(yellowColorMaterial);
                        genericNpc.questFinished = true;
                        genericNpc.canBeInteracted = true;
                        uiUpdater.UpdateDescription(quest, "All wolves killed. Return to npc.");
                    }
                }
                break;
        }
    }

    private void UpdateRemainingKills()
    {
        uiUpdater.UpdateDescription(quest, string.Format(REMAINING_KILLS_TEXT, killCounter));
    }
}
