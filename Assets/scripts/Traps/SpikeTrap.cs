using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Publisher
{
    private Observer observer;

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

    public override void AddObserver(Observer observer)
    {
        this.observer = observer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Player"))
        {
            observer.OnEvent(new EventDTO(EventType.PLAYER_DIED));
        }
    }
}
