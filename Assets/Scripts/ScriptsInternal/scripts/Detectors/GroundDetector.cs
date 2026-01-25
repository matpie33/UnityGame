using UnityEngine;

public class GroundDetector : MonoBehaviour
{

    private EventQueue eventQueue;
    public GameObject ground;
    private int collisionCount = 0;
    public bool isCollidingWithGround { get; private set; }

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionCount++;
        
        if (collisionCount == 1)
        {
            isCollidingWithGround = true;
            ground = other.gameObject;
            eventQueue.SubmitEvent(new EventDTO(EventType.GROUND_DETECTED, ground));
        }

    }

    private void OnTriggerExit(Collider other)
    {
        collisionCount--;
        if (collisionCount == 0)
        {

            isCollidingWithGround = false;
            eventQueue.SubmitEvent(new EventDTO(EventType.STARTED_FALLING, null));
            ground = null;
        }
    }


}
