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
                GameObject gate = (GameObject)eventDTO.eventData;
                Vector3 lookAtPosition = gate.transform.position + Vector3.up * 2f;
                cam.transform.position = lookAtPosition + gate.transform.forward * 15;
                cam.transform.LookAt(lookAtPosition);
                Debug.Log("Focus");
                break;
        }
    }
}
