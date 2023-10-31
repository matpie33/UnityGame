using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpening : Observer
{
    private Animator animator;
    private EventQueue eventQueue;

    private void Start()
    {
        animator = GetComponent<Animator>();
        eventQueue = FindObjectOfType<EventQueue>();
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.LEVER_OPENED:
                animator.Play("Base Layer.open");
                break;
        }
    }

    public void SubmitGateOpenedEvent()
    {
        Invoke("SubmitEvent", 1);
    }

    private void SubmitEvent()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.GATE_OPENED, gameObject));
    }
}
