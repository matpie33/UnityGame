using UnityEngine;

public class WallPushButton : Interactable, Restorable
{
    [SerializeField]
    private GameObject wallToPush;

    [SerializeField]
    private float force;

    private EventQueue eventQueue;

    [SerializeField]
    private AnimationClip cameraAnimation;

    private Animator animator;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
        animator = wallToPush.GetComponent<Animator>();
    }

    public void PushWall()
    {
        animator.Play("BridgeLowering");
    }

    public override void Interact(Object data)
    {
        FindAnyObjectByType<GameManager>().SaveCheckpoint(this);
        eventQueue.SubmitEvent(new EventDTO(EventType.WALL_STARTS_FALLING, cameraAnimation));
    }

    public void RestoreState()
    {
        canBeInteracted = false;
        PushWall();
    }

}
