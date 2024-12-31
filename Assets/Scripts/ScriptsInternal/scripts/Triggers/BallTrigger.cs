using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject ballToTrigger;

    private EventQueue eventQueue;

    private bool alreadyTriggered;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyTriggered && other.CompareTag(Tags.PLAYER))
        {
            Rigidbody rg = ballToTrigger.GetComponent<Rigidbody>();
            rg.isKinematic = false;
            rg.AddForce(transform.forward * 2, ForceMode.Impulse);
            alreadyTriggered = true;
            eventQueue.SubmitEvent(new EventDTO(EventType.FALLING_BALL_TRIGGERED, ballToTrigger));
        }
    }
}
