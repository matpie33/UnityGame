using UnityEditor;
using UnityEngine;

public class EnemyInRangeDTO
{
    public GameObject enemy { get; private set; }
    public bool isInRange { get; private set; }

    public EnemyInRangeDTO(GameObject enemy, bool isInRange)
    {
        this.enemy = enemy;
        this.isInRange = isInRange;
    }
}
