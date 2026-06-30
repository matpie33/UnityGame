using UnityEngine;

public class EnemyInRangeDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag.Equals(Tags.ENEMY))
        {
            SetEnemyInRange(other, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.Equals(Tags.ENEMY))
        {
            SetEnemyInRange(other, false);
        }
    }

    private static void SetEnemyInRange(Collider collider, bool isInRange)
    {
        collider.gameObject.GetComponentInParent<Enemy>().isInRange = isInRange;
    }
}
