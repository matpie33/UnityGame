using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class UpdatePositionForLeftHandConstraint : Observer
{
    private GameObject objectPositionHolder;

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.LEVER_OPENING:
                GameObject objectToInteractWith = (GameObject)eventDTO.eventData;
                objectPositionHolder = objectToInteractWith
                    .GetComponentInChildren<Movable>()
                    .gameObject;
                break;
            case EventType.LEVER_OPENED:
                objectPositionHolder = null;
                break;
        }
    }

    void Update()
    {
        if (objectPositionHolder != null)
        {
            transform.position = objectPositionHolder.transform.position;
        }
    }
}
