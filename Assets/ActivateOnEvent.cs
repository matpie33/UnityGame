using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnEvent : Observer
{
    [SerializeField]
    private EventType activateEvent;

    [SerializeField]
    private List<EventType> deactivateEvents;

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        if (eventDTO.eventType.Equals(activateEvent))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (deactivateEvents.Contains(eventDTO.eventType))
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
