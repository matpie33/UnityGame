﻿using UnityEditor;
using UnityEngine;

public enum EventType
{
    PLAYER_DIED,
    ENEMY_IN_RANGE,
    OBJECT_NOW_IN_RANGE,
    OBJECT_OUT_OF_RANGE,
    LEVER_OPENING,
    BACKPACK_OPEN_CLOSE_EVENT,
    INTERACTION_DONE,
    GATE_OPENED,
    LEVER_OPENED,
    CHARACTER_LEVEL_UP,
    QUEST_RECEIVED,
    QUEST_STEP_COMPLETED,
    OBJECT_HP_DECREASE,
    NPC_QUEST_AVAILABLE,
    QUEST_CONFIRMATION_NEEDED,
    QUEST_CONFIRMATION_DONE,
    OBJECT_DESTROYED
}
