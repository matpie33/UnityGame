using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WallHitDetector : MonoBehaviour
{
    private EventQueue eventQueue;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.PLAYER_COLLIDED, null));
    }
}
