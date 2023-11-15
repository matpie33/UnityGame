using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionHint : Observer
{
    private TextMeshProUGUI textField;

    private bool ignoreEvents = false;

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.OBJECT_NOW_IN_RANGE:
                if (ignoreEvents)
                {
                    return;
                }
                GameObject eventObject = (GameObject)eventDTO.eventData;
                textField.enabled = true;

                if (eventObject.GetComponent<LockedDoor>() != null)
                {
                    LockedDoor lockedDoor = eventObject.GetComponent<LockedDoor>();
                    if (lockedDoor.isOpened)
                    {
                        textField.enabled = false;
                        return;
                    }
                    if (lockedDoor.PlayerHasKey())
                    {
                        textField.text = $"Press {ActionKeys.INTERACT} to open. ";
                    }
                    else
                    {
                        textField.text = "Locked door";
                    }
                }
                else if (eventObject.GetComponent<Pickable>() != null)
                {
                    textField.text = $"Press {ActionKeys.INTERACT} to pickup. ";
                }
                else if (eventObject.GetComponent<Pullable>() != null)
                {
                    textField.text = $"Press {ActionKeys.INTERACT} to pull. ";
                }
                else if (
                    eventObject.GetComponent<NpcInteractions>() != null
                    && eventObject.GetComponent<NpcInteractions>().enabled
                ) //TODO how to do it better
                {
                    textField.text = $"Press {ActionKeys.INTERACT} to talk. ";
                }
                else
                {
                    textField.enabled = false;
                }

                break;

            case EventType.OBJECT_OUT_OF_RANGE:
            case EventType.INTERACTION_DONE:
                textField.enabled = false;
                break;
            case EventType.BACKPACK_OPEN_CLOSE_EVENT:
                bool isOpened = (bool)eventDTO.eventData;
                ignoreEvents = isOpened;
                textField.enabled = false;
                break;
        }
    }

    private void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        textField.enabled = false;
    }
}
