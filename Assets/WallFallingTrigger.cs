using UnityEngine;

public class WallFallingTrigger : MonoBehaviour
{
    [SerializeField]
    private Rigidbody wallToTrigger;

    [SerializeField]
    private float force;

    private bool hasFallen;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasFallen && other.CompareTag(Tags.PLAYER))
        {
            wallToTrigger.AddForceAtPosition(
                (transform.position - wallToTrigger.transform.position).normalized * force,
                wallToTrigger.transform.position,
                ForceMode.Impulse
            );
            hasFallen = true;
        }
    }
}
