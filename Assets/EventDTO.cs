using UnityEditor;
using UnityEngine;

public class EventDTO
{
    public EventType eventType { get; private set; }

    public bool isAttacking { get; private set; }

    private EventDTO() { }

    public static EventDTO createEventPlayerAttack(bool isAttacking)
    {
        EventDTO eventDTO = new EventDTO();
        eventDTO.eventType = EventType.PLAYER_ATTACK;
        eventDTO.isAttacking = isAttacking;
        return eventDTO;
    }
}
