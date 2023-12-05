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

    private void Start()
    {
        npcSounds = GetComponent<NpcSounds>();
        killCounter = killsNeeded;
        uiUpdater = FindObjectOfType<UIUpdater>();
        quest = GetComponent<GenericNpc>().quest;
        UpdateRemainingKills();
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
                }
                break;
            case EventType.ENEMY_KILLED:
                GameObject data = (GameObject)eventDTO.eventData;
                EnemyType enemyType = data.GetComponent<Enemy>().enemyType;
                if (enemyType.Equals(this.enemyToKill))
                {
                    killCounter--;
                    UpdateRemainingKills();
                }
                break;
        }
    }

    private void UpdateRemainingKills()
    {
        uiUpdater.UpdateDescription(quest, string.Format(REMAINING_KILLS_TEXT, killCounter));
    }
}
