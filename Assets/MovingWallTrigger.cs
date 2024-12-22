using UnityEngine;

public class MovingWallTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject wallToTrigger;

    [SerializeField]
    private float delay;

    private void Start() { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PLAYER))
        {
            Invoke(nameof(PlayAnimation), delay);
        }
    }

    private void OnDrawGizmos()
    {
        float size = .5f;
        Gizmos.DrawCube(transform.position, new Vector3(size, size, size));
    }

    private void PlayAnimation()
    {
        wallToTrigger.GetComponent<Animator>().Play("Base Layer.Move");
    }
}
