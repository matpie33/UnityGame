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
        eventQueue = FindObjectOfType<EventQueue>();
    }

    public override void Interact(Object data)
    {
        animator.Play("Base Layer.open");
    }

    public void LeverOpened()
    {
        Invoke("SubmitEvent", 0.5f);
    }

    private void SubmitEvent()
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.LEVER_OPENED, gateToOpen));
    }
}
