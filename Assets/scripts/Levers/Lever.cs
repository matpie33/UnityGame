using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Pullable
{
    private Animator animator;

    [SerializeField]
    private GameObject gateToOpen;

    private EventQueue eventQueue;

    private void Start()
    {
        animator = GetComponent<Animator>();
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    public override void Interact(Object data)
    {
        Invoke(nameof(SubmitEvent), 0.5f);
    }

    public void SubmitEvent()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.LEVER_OPENED, gateToOpen));
    }
}
