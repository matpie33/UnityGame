using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenEmpty : MonoBehaviour
{
    private EventQueue eventQueue;

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
    }

    void Update()
    {
        if (transform.childCount == 0)
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_DESTROYED, gameObject));
            Destroy(gameObject);
        }
    }
}
