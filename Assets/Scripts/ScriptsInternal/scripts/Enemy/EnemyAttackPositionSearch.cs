using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAttackPositionSearch : MonoBehaviour
{
    private Dictionary<Enemy, Direction> enemyToPositionDictionary = new();

    private CharacterController characterController;

    public static EnemyAttackPositionSearch INSTANCE;

    private void Awake()
    {
        INSTANCE = this;
    }

    private void Start()
    {
        characterController = FindAnyObjectByType<CharacterController>();
    }

    private Vector3 GetPositionForEnemy(Enemy enemy, Direction direction)
    {
        Vector3 directionVector =
            direction.Equals(Direction.FORWARD) || direction.Equals(Direction.BACKWARD)
                ? characterController.transform.forward
                : characterController.transform.right;
        return characterController.transform.position
            + (0.5f + characterController.playerColliderRadius)
                * (directionVector * Mathf.Sign((int)direction));
    }

    public Vector3 GetPositionForEnemy(Enemy enemy)
    {
        if (enemyToPositionDictionary.ContainsKey(enemy))
        {
            Direction positionNumber = enemyToPositionDictionary[enemy];
            return GetPositionForEnemy(enemy, positionNumber);
        }
        else
        {
            List<Direction> freeDirections = new List<Direction>();
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (!enemyToPositionDictionary.Values.ToList().Contains(direction))
                {
                    freeDirections.Add(direction);
                }
            }
            if (freeDirections.Count == 0)
            {
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    Vector3 position = GetPositionForEnemy(enemy, direction);
                    float myDistance = Vector3.Distance(enemy.transform.position, position);
                    Enemy other = enemyToPositionDictionary
                        .Where(entry => entry.Value.Equals(direction))
                        .First()
                        .Key;
                    float hisDistance = Vector3.Distance(other.transform.position, position);

                    if (myDistance < hisDistance)
                    {
                        enemyToPositionDictionary.Remove(other);
                        enemyToPositionDictionary.Add(enemy, direction);
                        other.ClearPath();
                        return position;
                    }
                }

                return Vector3.zero;
            }
            float localMinDistance = Mathf.Infinity;
            Vector3 targetPosition = Vector3.zero;
            Direction targetDirection = Direction.FORWARD;
            foreach (Direction direction in freeDirections)
            {
                Vector3 position = GetPositionForEnemy(enemy, direction);
                float distance = Vector3.Distance(enemy.transform.position, position);
                if (distance < localMinDistance)
                {
                    targetDirection = direction;
                    targetPosition = position;
                    localMinDistance = distance;
                }
            }

            enemyToPositionDictionary.Add(enemy, targetDirection);
            return targetPosition;
        }
    }

    public void EnemyGone(Enemy enemy)
    {
        enemyToPositionDictionary.Remove(enemy);
    }

    internal void ClearEnemyPlaceIfWasNearPlayer(Enemy enemy)
    {
        enemyToPositionDictionary.Remove(enemy);
    }
}
