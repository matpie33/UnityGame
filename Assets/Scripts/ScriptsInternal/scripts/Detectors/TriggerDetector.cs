using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    public bool isColliding { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            isColliding = false;
        }
    }
}
