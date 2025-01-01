using System.Collections;
using UnityEngine;

public class DestroyOnHit : MonoBehaviour
{
    [SerializeField]
    private float timeout;

    private Collider colliderComponent;

    private Rigidbody rb;

    void Start()
    {
        StartCoroutine(DestroyAfterTimeout());
        colliderComponent = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator DestroyAfterTimeout()
    {
        yield return new WaitForSeconds(timeout);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            colliderComponent.isTrigger = false;
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Invoke(nameof(DestroyThis), 1f);
    }

    private void DestroyThis()
    {
        Destroy(gameObject);
    }
}
