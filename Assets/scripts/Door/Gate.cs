using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Observer
{
    private EventQueue eventQueue;
    private Animator animator;

    private void Awake()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
        animator = GetComponent<Animator>();
    }

    public void DoOpen()
    {
        animator.Play("Base.fence_gate_opened");
    }

    public void SubmitGateOpenedEvent()
    {
        Invoke(nameof(SubmitEvent), 1);
    }

    private void SubmitEvent()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.GATE_OPENED, gameObject));
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.LEVER_OPENED:
                LeverOpenedEventDTO eventData = (LeverOpenedEventDTO)eventDTO.eventData;
                GameObject gate = eventData.gate;
                if (gate.Equals(gameObject))
                {
                    animator.enabled = true;
                    animator.Play("Base.open");
                }
                break;
        }
    }
}
