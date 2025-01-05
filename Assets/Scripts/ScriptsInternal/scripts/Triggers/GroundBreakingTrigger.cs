using UnityEngine;

public class GroundBreakingTrigger : MonoBehaviour
{
    private GroundBreakingExecutor groundBreakingExecutor;
    private Animator animator;
    private bool wasTriggered;

    private void Start()
    {
        groundBreakingExecutor = GetComponentInParent<GroundBreakingExecutor>();
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!wasTriggered && collision.collider.CompareTag(Tags.PLAYER))
        {
            wasTriggered = true;
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
