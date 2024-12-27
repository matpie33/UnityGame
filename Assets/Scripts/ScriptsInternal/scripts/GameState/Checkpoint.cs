using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameManager gameManager;
    private EventQueue eventQueue;
    public List<Gate> openedGates { get; private set; }

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        eventQueue = FindAnyObjectByType<EventQueue>();
        openedGates = new();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one);
    }

    public void SaveCheckpoint()
    {
        gameManager.SaveCheckpoint(this);
        eventQueue.SubmitEvent(new EventDTO(EventType.CHECKPOINT_ENTERED, gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (
            !gameManager.LastCheckpointPosition().Equals(transform.position)
            && other.CompareTag(Tags.PLAYER)
        )
        {
            SaveCheckpoint();
        }
    }
}
