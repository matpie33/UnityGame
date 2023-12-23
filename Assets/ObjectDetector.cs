using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    public bool objectInFrontDetected;
    private EventQueue eventQueue;

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
    }

    private void OnTriggerEnter(Collider other)
    {
        objectInFrontDetected = true;
        eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_NOW_IN_RANGE, other.gameObject));
    }

    private void OnTriggerExit(Collider other)
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_OUT_OF_RANGE, null));
        objectInFrontDetected = false;
    }
}
