using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInRangeDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Tags.ENEMY))
        {
            SetEnemyInRange(other, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals(Tags.ENEMY))
        {
            SetEnemyInRange(other, false);
        }
    }

    private static void SetEnemyInRange(Collider collider, bool isInRange)
    {
        collider.gameObject.GetComponent<Enemy>().isInRange = isInRange;
    }
}
