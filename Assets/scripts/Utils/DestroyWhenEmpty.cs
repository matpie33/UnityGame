using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenEmpty : MonoBehaviour
{
    private EventQueue eventQueue;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    void Update()
    {
        int activeChildCount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }
        bool hasChildren = transform.childCount != 0;
        if (activeChildCount == 0 || !hasChildren)
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_DESTROYED, gameObject));
            gameObject.SetActive(false);
            if (!hasChildren)
            {
                Destroy(gameObject);
            }
        }
    }
}
