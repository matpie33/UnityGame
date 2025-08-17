using UnityEngine;
using UnityEngine.Serialization;

public class Lever : Pullable
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
            SaveCheckpoint();
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
}
