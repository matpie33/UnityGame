using UnityEngine;

public class ObstacleDetector : MonoBehaviour
{
    public bool obstacleInFrontDetected
    {
        get { return collisionCount > 0; }
    }

    public GameObject obstacle;

    private int collisionCount;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            collisionCount++;
            obstacle = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            collisionCount--;
        }
        if (collisionCount == 0)
        {
            obstacle = null;
        }
    }
}
