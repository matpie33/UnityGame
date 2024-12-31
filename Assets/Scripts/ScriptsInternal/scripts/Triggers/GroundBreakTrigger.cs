using UnityEngine;

public class GroundBreakTrigger : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (
            collision.collider.GetType() == typeof(SphereCollider)
            && collision.gameObject.GetComponent<Rigidbody>() != null
        )
        {
            GameObject fractures = gameObject.transform.parent.Find("Fractures").gameObject;
            fractures.SetActive(true);
            foreach (Transform child in fractures.transform)
            {
                Rigidbody rb = child.GetComponent<Rigidbody>();
                rb.AddExplosionForce(10000, rb.transform.position, 500);
                Destroy(child.GetComponent<Collider>());
            }
            Destroy(gameObject);
        }
    }
}
