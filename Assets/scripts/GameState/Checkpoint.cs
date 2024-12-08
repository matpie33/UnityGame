using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager gameManager;
    private EventQueue eventQueue;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (
            !gameManager.LastCheckpointPosition().Equals(transform.position)
            && other.CompareTag(Tags.PLAYER)
        )
        {
            gameManager.SaveCheckpoint(gameObject);
            eventQueue.SubmitEvent(new EventDTO(EventType.CHECKPOINT_ENTERED, gameObject));
        }
    }
}
