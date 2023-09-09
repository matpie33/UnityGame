using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour, Observer
{
    private bool isPlayerAttacking;

    private List<Enemy> enemies;

    private float minDistanceToAttack = 2;

    private CharacterController characterController;

    private ISet<Enemy> objectsToDelete = new HashSet<Enemy>();

    private void Start()
    {
        enemies = FindObjectsOfType<Enemy>().ToList<Enemy>();
        characterController = FindObjectOfType<CharacterController>();
        characterController.AddObserver(this);
    }

    public void SetPlayerIsAttacking(bool isAttacking)
    {
        isPlayerAttacking = isAttacking;
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
            if (isPlayerAttacking && distance < minDistanceToAttack)
            {
                enemy.DecreaseHealth(characterController.playerState.attackPower);
                isPlayerAttacking = false;
            }
            if (enemy.GetIsAttacking())
            {
                characterController.DecreaseHealth(enemy.getAttackPower());
            }
        }
        foreach (Enemy e in objectsToDelete)
        {
            enemies.Remove(e);
        }
    }

    public void Notify(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.PLAYER_ATTACK:
                isPlayerAttacking = eventDTO.isAttacking;
                break;
        }
    }
}
