using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKill : MonoBehaviour
{
    private EventQueue eventQueue;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Tags.PLAYER))
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.PLAYER_DIED, null));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals(Tags.PLAYER))
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.PLAYER_DIED, null));
        }
    }
}
