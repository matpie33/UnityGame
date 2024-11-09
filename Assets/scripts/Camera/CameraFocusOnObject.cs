using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusOnObject : Observer
{
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.LEVER_OPENING:
                GameObject interactable = (GameObject)eventDTO.eventData;
                GetComponent<Camera>().transform.position =
                    interactable.transform.position + interactable.transform.right * 5;
                cam.transform.LookAt(interactable.transform.position);
                break;
            case EventType.LEVER_OPENED:
                LeverOpenedEventDTO eventData = (LeverOpenedEventDTO)eventDTO.eventData;
                GameObject gate = eventData.gate;
                cam.transform.position = eventData.cameraPositionToSet;
                cam.transform.LookAt(gate.transform.position);
                break;
        }
    }
}
