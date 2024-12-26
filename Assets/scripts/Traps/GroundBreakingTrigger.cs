using System.Collections;
using UnityEngine;

public class GroundBreakingTrigger : MonoBehaviour
{
    private GroundBreakingExecutor groundBreakingExecutor;
    private Animator animator;

    private void Start()
    {
        groundBreakingExecutor = GetComponentInParent<GroundBreakingExecutor>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(Tags.PLAYER))
        {
            Invoke(nameof(BreakGround), .3f);
        }
    }

    private void BreakGround()
    {
        groundBreakingExecutor.Execute();
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        animator.Play("Base Layer.BreakingGround");
    }
}
