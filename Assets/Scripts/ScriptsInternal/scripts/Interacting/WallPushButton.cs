using UnityEngine;

public class WallPushButton : Interactable
{
    [SerializeField]
    private GameObject wallToPush;

    private Rigidbody rb;

    [SerializeField]
    private float force;

    private EventQueue eventQueue;

    [SerializeField]
    private AnimationClip cameraAnimation;

    private void Start()
    {
        rb = wallToPush.GetComponent<Rigidbody>();
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    public void PushWall()
    {
        rb.AddForce(transform.forward * force * rb.mass);
    }

    public override void Interact(Object data)
    {
        eventQueue.SubmitEvent(new EventDTO(EventType.WALL_STARTS_FALLING, cameraAnimation));
    }
}
