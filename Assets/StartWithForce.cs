using UnityEngine;

public class StartWithForce : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float forceCoefficient;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * rb.mass * forceCoefficient, ForceMode.Impulse);
        Invoke(nameof(DestroyMe), 4f);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
