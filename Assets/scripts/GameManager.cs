using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<Enemy> enemies;

    private float minDistanceToAttack = 2;

    private CharacterController characterController;

    private ISet<Enemy> objectsToDelete = new HashSet<Enemy>();

    private void Start()
    {
        enemies = FindObjectsOfType<Enemy>().ToList<Enemy>();
        characterController = FindObjectOfType<CharacterController>();
    }

    void Update()
    {
        objectsToDelete.Clear();
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.IsAlive())
            {
                Destroy(enemy.gameObject);
                objectsToDelete.Add(enemy);
                characterController.AddExperience(enemy.experienceValue);
                continue;
            }
            float distance = Vector3.Distance(
                enemy.navMeshAgent.transform.position,
                characterController.transform.position
            );
            if (characterController.IsAttacking() && distance < minDistanceToAttack)
            {
                enemy.DecreaseHealth(characterController.playerState.attackPower);
            }
            if (enemy.GetIsAttacking())
            {
                characterController.DecreaseHealth(enemy.getAttackPower());
            }
        }
        characterController.attackEventChecked();
        foreach (Enemy e in objectsToDelete)
        {
            enemies.Remove(e);
        }
    }
}
