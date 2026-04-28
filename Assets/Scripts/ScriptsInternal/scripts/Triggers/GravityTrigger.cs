using UnityEngine;

public class GravityTrigger : MonoBehaviour
{
    private Rigidbody rigidBody;

    [SerializeField]
    public float Delay;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.PLAYER))
        {
            Invoke(nameof(EnableGravity), Delay);
            
        }
    }

    private void EnableGravity ()
    {
        rigidBody.isKinematic = false;
    }

}
