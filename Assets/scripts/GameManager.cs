using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Observer
{
    private List<ObjectWithHealth> objectsWithHealth;

    private CharacterController characterController;

    private ISet<ObjectWithHealth> objectsToDelete = new HashSet<ObjectWithHealth>();

    private StatsToValuesConverter statsToValuesConverter;

    [SerializeField]
    private GameObject gameOverText;

    private EventQueue eventQueue;

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.PLAYER_DIED:
                DoGameOver();
                break;
        }
    }

    private void DoGameOver()
    {
        Time.timeScale = 0;
        gameOverText.SetActive(true);
    }

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
        gameOverText.SetActive(false);
        objectsWithHealth = FindObjectsOfType<ObjectWithHealth>().ToList<ObjectWithHealth>();
        statsToValuesConverter = new StatsToValuesConverter();
        characterController = FindObjectOfType<CharacterController>();
    }

    void Update()
    {
        objectsToDelete.Clear();
        foreach (ObjectWithHealth aliveObject in objectsWithHealth)
        {
            Enemy enemy = null;
            if (aliveObject.aliveObjectType.Equals(TypeOfObjectWithHealth.ENEMY))
            {
                enemy = aliveObject.GetComponent<Enemy>();
            }
            if (!aliveObject.IsAlive())
            {
                Destroy(aliveObject.gameObject);
                objectsToDelete.Add(aliveObject);
                if (enemy != null)
                {
                    characterController.AddExperience(enemy.experienceValue);
                }
                continue;
            }
            if (enemy != null)
            {
                HandleEnemy(aliveObject, enemy);
            }
        }
        characterController.attackEventChecked();
        foreach (ObjectWithHealth e in objectsToDelete)
        {
            objectsWithHealth.Remove(e);
        }
    }

    private void HandleEnemy(ObjectWithHealth aliveObject, Enemy enemy)
    {
        if (characterController.IsAttacking() && enemy.isInRange)
        {
            aliveObject.DecreaseHealth(
                statsToValuesConverter.ConvertStrengthToHealthDecreaseValue(
                    characterController.playerState.strength
                )
            );
            eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_HP_DECREASE, aliveObject));
        }
        if (enemy.GetIsAttacking())
        {
            characterController.DecreaseHealth(
                statsToValuesConverter.ConvertDefenceToPlayerHealthDecrease(
                    characterController.playerState.defence,
                    enemy.getAttackPower()
                )
            );
            if (!characterController.healthState.IsAlive())
            {
                DoGameOver();
            }
        }
    }
}
