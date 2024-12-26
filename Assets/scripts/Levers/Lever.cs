using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Pullable
{
    private Animator animator;

    [field: SerializeField]
    public Gate gateToOpen { get; private set; }

    private EventQueue eventQueue;

    [SerializeField]
    private Vector3 cameraPosition;

    private Checkpoint checkpoint;

    private void Start()
    {
        animator = GetComponent<Animator>();
        eventQueue = FindAnyObjectByType<EventQueue>();
        checkpoint = FindAnyObjectByType<Checkpoint>();
    }

    public override void Interact(Object data)
    {
        Invoke(nameof(SubmitEvent), 0.5f);
    }

    private void SaveCheckpoint()
    {
        GameManager.gameStateManager.AddOpenedLever(this);
        Checkpoint newCheckpoint = Instantiate(checkpoint);
        Destroy(newCheckpoint.GetComponent<Collider>());
        newCheckpoint.transform.position =
            FindAnyObjectByType<CharacterController>().transform.position;
        newCheckpoint.SaveCheckpoint();
        Destroy(newCheckpoint);
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        if (
            eventDTO.eventType.Equals(EventType.GATE_OPENED)
            && eventDTO.eventData.Equals(gateToOpen.gameObject)
        )
        {
            SaveCheckpoint();
        }
    }

    public void SubmitEvent()
    {
        eventQueue.SubmitEvent(
            new EventDTO(
                EventType.LEVER_OPENED,
                new LeverOpenedEventDTO(gateToOpen.gameObject, cameraPosition)
            )
        );
    }
}
