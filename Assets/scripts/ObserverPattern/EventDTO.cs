using UnityEditor;
using UnityEngine;

public class EventDTO
{
    public EventType eventType { get; private set; }

    public EventDTO(EventType eventType)
    {
        this.eventType = eventType;
    }
}
