using UnityEngine;
using UnityEngine.Serialization;

public class Lever : Pullable, Restorable
{
    private Animator animator;

    [field: SerializeField]
    public Gate gateToOpen { get; private set; }

    private EventQueue eventQueue;

    [SerializeField]
    [FormerlySerializedAs("cameraPosition")]
    private GameObject lookAtGateCameraPosition;

    [field: SerializeField]
    public GameObject lookAtLeverCameraPosition { get; private set; }

    private Checkpoint checkpoint;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        eventQueue = FindAnyObjectByType<EventQueue>();
        checkpoint = FindAnyObjectByType<Checkpoint>();
    }

    public override void Interact(Object data)
    {
        PlayOpenAnimation();
    }

    

    public void SubmitEvent()
    {
        eventQueue.SubmitEvent(
            new EventDTO(
                EventType.LEVER_OPENED,
                new ObjectWithPositionDTO(
                    gateToOpen.gameObject,
                    lookAtGateCameraPosition.transform.position
                )
            )
        );
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        if (
            eventDTO.eventType.Equals(EventType.GATE_OPENED)
            && eventDTO.eventData.Equals(gateToOpen.gameObject)
        )
        {
            FindAnyObjectByType<GameManager>().SaveCheckpoint(this);
        }
    }

    public void PlayOpenAnimation()
    {
        animator.Play("Base Layer.open");
    }

    public void SwitchToOpenedAnimation()
    {
        animator.Play("Base Layer.Opened");
    }

    public void RestoreState()
    {
        canBeInteracted = false;
        SwitchToOpenedAnimation();
        gateToOpen.DoOpen();
    }
}
