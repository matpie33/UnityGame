using System;
using UnityEngine;

public class PlayerKillTrigger : MonoBehaviour
{
    private EventQueue eventQueue;

    private Rigidbody rigidBody;

    [SerializeField]
    private float minSpeedToDefeatPlayer;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
        rigidBody = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (
            collision.collider.tag.Equals(Tags.PLAYER) && (minSpeedToDefeatPlayer == 0 || rigidBody.linearVelocity.magnitude >= minSpeedToDefeatPlayer)

        )
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.PLAYER_DIED, null));
        }
    }
}
