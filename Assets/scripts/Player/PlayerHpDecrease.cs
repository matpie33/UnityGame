using UnityEngine;

public class PlayerHpDecrease : MonoBehaviour
{
    private EventQueue eventQueue;

    [SerializeField]
    private int hpDecreaseValue;

    private void Start()
    {
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(Tags.PLAYER))
        {
            collision.collider.gameObject
                .GetComponent<ObjectWithHealth>()
                .DecreaseHealth(hpDecreaseValue);
        }
    }
}
