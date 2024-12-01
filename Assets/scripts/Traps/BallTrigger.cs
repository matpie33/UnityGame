using System.Collections;
using UnityEngine;

public class BallTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject ballToTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            Rigidbody rg = ballToTrigger.GetComponent<Rigidbody>();
            rg.isKinematic = false;
            rg.AddForce(transform.forward * 2, ForceMode.Impulse);
        }
    }
}
