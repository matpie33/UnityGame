using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnEvent : Observer
{
    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.QUEST_CONFIRMATION_NEEDED:
                transform.GetChild(0).gameObject.SetActive(true);
                break;
            case EventType.QUEST_CONFIRMATION_DONE:
                transform.GetChild(0).gameObject.SetActive(false);
                break;
        }
    }
}
