using System;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class EventDTO
{
    public EventType eventType { get; private set; }

    public Object eventData { get; private set; }

    public EventDTO(EventType eventType, Object eventData)
    {
        this.eventType = eventType;
        this.eventData = eventData;
    }
}
