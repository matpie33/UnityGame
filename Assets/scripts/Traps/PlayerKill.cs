using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKill : Publisher
{
    private void Start()
    {
        AddObserver(FindObjectOfType<GameManager>());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals(Tags.PLAYER))
        {
            foreach (Observer observer in observers)
            {
                observer.OnEvent(new EventDTO(EventType.PLAYER_DIED, null));
            }
        }
    }
}
