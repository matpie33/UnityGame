using System.Collections;
using UnityEngine;

public class GroundBreakingTrigger : MonoBehaviour
{
    private GroundBreakingExecutor groundBreakingExecutor;

    private void Start()
    {
        groundBreakingExecutor = GetComponentInParent<GroundBreakingExecutor>();
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
        Destroy(gameObject);
    }
}
