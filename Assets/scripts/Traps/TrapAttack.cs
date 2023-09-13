using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAttack : Publisher
{
    private bool isRunning = true;

    private Animator animator;

    private bool isInAttack;

    [SerializeField]
    private float timeInAttack;

    [SerializeField]
    private float timeInIdle;

    private void Start()
    {
        isInAttack = true;
        animator = transform.parent.GetComponent<Animator>();
        StartCoroutine(DoAnimations());
    }

    private IEnumerator DoAnimations()
    {
        float waitTime = timeInAttack;
        while (isRunning)
        {
            yield return new WaitForSeconds(waitTime);
            if (isInAttack)
            {
                animator.Play("Base Layer.GoIdle");
                waitTime = timeInIdle;
            }
            else
            {
                animator.Play("Base Layer.DoHarm");
                waitTime = timeInAttack;
            }
            isInAttack = !isInAttack;
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
