using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Publisher
{
    private bool isRunning = true;

    private Animator animator;

    private bool isDown;

    [SerializeField]
    private float timeBeingUp;

    [SerializeField]
    private float timeBeingDown;

    private void Start()
    {
        isDown = true;
        animator = transform.parent.GetComponent<Animator>();
        StartCoroutine(DoAnimations());
    }

    private IEnumerator DoAnimations()
    {
        float waitTime = timeBeingDown;
        while (isRunning)
        {
            yield return new WaitForSeconds(waitTime);
            if (isDown)
            {
                animator.Play("L1.GoDown");
                waitTime = timeBeingDown;
            }
            else
            {
                animator.Play("L1.GoUp");
                waitTime = timeBeingUp;
            }
            isDown = !isDown;
        }
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
