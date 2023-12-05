using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquireQuest : MonoBehaviour
{
    private EventQueue eventQueue;

    [SerializeField]
    private Quest quest;

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(Tags.PLAYER))
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.QUEST_ACCEPTED, quest));
            Destroy(gameObject);
        }
    }
}
