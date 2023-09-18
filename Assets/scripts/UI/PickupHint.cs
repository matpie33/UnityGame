using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PickupHint : Observer
{
    private TextMeshProUGUI textField;

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.OBJECT_NOW_IN_RANGE:
                Interactable interactable = (Interactable)eventDTO.eventData;
                if (typeof(Pickable).IsAssignableFrom(interactable.GetType()))
                {
                    textField.text = $"Press {ActionKeys.INTERACT} to pickup. ";
                }
                else if (typeof(Pullable).IsAssignableFrom(interactable.GetType()))
                {
                    textField.text = $"Press {ActionKeys.INTERACT} to pull. ";
                }
                textField.enabled = true;
                break;

            case EventType.OBJECT_OUT_OF_RANGE:
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
