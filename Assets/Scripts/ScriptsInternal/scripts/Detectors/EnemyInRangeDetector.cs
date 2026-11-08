using UnityEngine;

public class EnemyInRangeDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Enemy>() != null)
        {
            SetEnemyInRange(other, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Enemy>() != null)
        {
            SetEnemyInRange(other, false);
        }
    }

    private static void SetEnemyInRange(Collider collider, bool isInRange)
    {
        collider.gameObject.GetComponentInParent<Enemy>().isInRange = isInRange;
    }
}
