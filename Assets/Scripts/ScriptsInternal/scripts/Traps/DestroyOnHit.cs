using System.Collections;
using UnityEngine;

public class DestroyOnHit : MonoBehaviour
{
    [SerializeField]
    private float timeout;

    void Start()
    {
        StartCoroutine(DestroyAfterTimeout());
    }

    private IEnumerator DestroyAfterTimeout()
    {
        yield return new WaitForSeconds(timeout);
        if (gameObject != null)
        {
            Destroy(gameObject);
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
