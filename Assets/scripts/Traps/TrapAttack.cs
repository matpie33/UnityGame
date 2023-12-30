using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAttack : MonoBehaviour
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
        float startDelay = Random.value * 2;
        yield return new WaitForSeconds(startDelay);
        float waitTime;
        while (isRunning)
        {
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
            yield return new WaitForSeconds(waitTime);
            isInAttack = !isInAttack;
        }
    }
}
