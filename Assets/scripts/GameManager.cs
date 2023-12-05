using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Observer
{
    public List<ObjectWithHealth> objectsWithHealth { get; private set; }

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

    private void Awake()
    {
        objectsWithHealth = FindObjectsOfType<ObjectWithHealth>().ToList();
    }

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
        gameOverText.SetActive(false);

        statsToValuesConverter = new StatsToValuesConverter();
        characterController = FindObjectOfType<CharacterController>();
    }

    void Update()
    {
        objectsToDelete.Clear();
        foreach (ObjectWithHealth objectWithHealth in objectsWithHealth)
        {
            TypeOfObjectWithHealth objectType = objectWithHealth.type;
            if (!objectWithHealth.IsAlive())
            {
                if (objectType.Equals(TypeOfObjectWithHealth.PLAYER))
                {
                    DoGameOver();
                }
                else if (objectType.Equals(TypeOfObjectWithHealth.ENEMY))
                {
                    if (objectWithHealth.gameObject.activeSelf)
                    {
                        characterController.AddExperience(
                            objectWithHealth.GetComponent<Enemy>().experienceValue
                        );
                    }
                    if (objectWithHealth.GetComponentInParent<QuestObject>())
                    {
                        objectWithHealth.gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(objectWithHealth.gameObject);
                        objectsToDelete.Add(objectWithHealth);
                    }
                    eventQueue.SubmitEvent(
                        new EventDTO(EventType.ENEMY_KILLED, objectWithHealth.gameObject)
                    );
                }
                else if (objectType.Equals(TypeOfObjectWithHealth.NPC))
                {
                    eventQueue.SubmitEvent(
                        new EventDTO(EventType.NPC_DIED, objectWithHealth.gameObject)
                    );
                }
                continue;
            }
            if (objectType.Equals(TypeOfObjectWithHealth.ENEMY))
            {
                HandleEnemy(objectWithHealth);
            }
        }
        characterController.attackEventChecked();
        foreach (ObjectWithHealth e in objectsToDelete)
        {
            objectsWithHealth.Remove(e);
        }
    }

    private void HandleEnemy(ObjectWithHealth enemyObject)
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (characterController.IsAttacking() && enemy.isInRange)
        {
            enemyObject.DecreaseHealth(
                statsToValuesConverter.ConvertDefenceToHealthDecrease(
                    enemyObject.stats.defence,
                    characterController.GetStats().strength
                )
            );
            eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_HP_DECREASE, enemyObject));
        }
        if (enemy.GetIsAttacking())
        {
            ObjectWithHealth attackTarget = enemy.attackedPerson;
            int defence = attackTarget.stats.defence;
            attackTarget.DecreaseHealth(
                statsToValuesConverter.ConvertDefenceToHealthDecrease(
                    defence,
                    enemyObject.stats.strength
                )
            );
            eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_HP_DECREASE, attackTarget));
        }
    }
}
