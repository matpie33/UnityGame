using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Observer
{
    private List<Enemy> enemies;

    private float minDistanceToAttack = 2;

    private CharacterController characterController;

    private ISet<Enemy> objectsToDelete = new HashSet<Enemy>();

    private StatsToValuesConverter statsToValuesConverter;

    [SerializeField]
    private GameObject gameOverText;

    private List<Publisher> publishers;

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.PLAYER_DIED:
                Time.timeScale = 0;
                gameOverText.SetActive(true);
                break;
        }
    }

    private void Start()
    {
        gameOverText.SetActive(false);
        publishers = FindObjectsOfType<Publisher>().ToList<Publisher>();
        Observer[] observers = FindObjectsOfType<Observer>();
        foreach (Publisher publisher in publishers)
        {
            foreach (Observer observer in observers)
            {
                publisher.AddObserver(observer);
            }
        }
        enemies = FindObjectsOfType<Enemy>().ToList<Enemy>();
        statsToValuesConverter = GetComponent<StatsToValuesConverter>();
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
            if (characterController.IsAttacking() && enemy.isInRange)
            {
                enemy.DecreaseHealth(
                    statsToValuesConverter.ConvertStrengthToHealthDecreaseValue(
                        characterController.playerState.strength
                    )
                );
            }
            if (enemy.GetIsAttacking())
            {
                characterController.DecreaseHealth(
                    statsToValuesConverter.ConvertDefenceToPlayerHealthDecrease(
                        characterController.playerState.defence,
                        enemy.getAttackPower()
                    )
                );
            }
        }
        characterController.attackEventChecked();
        foreach (Enemy e in objectsToDelete)
        {
            enemies.Remove(e);
        }
    }
}
