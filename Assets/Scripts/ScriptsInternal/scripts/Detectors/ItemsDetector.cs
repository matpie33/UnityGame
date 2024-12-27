using UnityEngine;

public class ItemsDetector : MonoBehaviour
{
    public bool objectInFrontDetected;
    private EventQueue eventQueue;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    private void OnTriggerEnter(Collider other)
    {
        objectInFrontDetected = true;
        eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_NOW_IN_RANGE, other.gameObject));
    }

    private void OnTriggerExit(Collider other)
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_OUT_OF_RANGE, other.gameObject));
        objectInFrontDetected = false;
    }
}
