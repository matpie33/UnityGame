using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Observer
{
    public List<ObjectWithHealth> objectsWithHealth { get; private set; }

    private CharacterController characterController;

    private ISet<ObjectWithHealth> objectsToDelete = new HashSet<ObjectWithHealth>();

    private StatsToValuesConverter statsToValuesConverter;

    [SerializeField]
    private GameObject gameOverText;

    private EventQueue eventQueue;

    public static GameStateManager gameStateManager = new GameStateManager();

    [SerializeField]
    private float minDistanceToFocusOnEnemy;

    [SerializeField]
    private float maxVerticalDistanceToFocusOnEnemies;

    private CameraController cameraController;

    private EnemyAttackPositionSearch enemyAttackPositionSearch;

    public InterruptableAnimationsHandler interruptableAnimationsHandler { get; set; }

    private void OnApplicationQuit()
    {
        gameStateManager = new GameStateManager();
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.PLAYER_DIED:
                DoGameOver();
                break;
            case EventType.ENEMY_STUN:
                objectsWithHealth
                    .Where(o => o.type.Equals(TypeOfObjectWithHealth.ENEMY))
                    .Select(o => o.GetComponent<Enemy>())
                    .Where(o => o.isInRange)
                    .ToList()
                    .ForEach(u => u.Stun((float)eventDTO.eventData));
                break;
        }
    }

    public Vector3 LastCheckpointPosition()
    {
        return gameStateManager.checkpointData != null
            ? gameStateManager.checkpointData.position
            : Vector3.zero;
    }

    private void DoGameOver()
    {
        Time.timeScale = 0;
        gameOverText.SetActive(true);
    }

    private void Awake()
    {
        objectsWithHealth = FindObjectsByType<ObjectWithHealth>(FindObjectsSortMode.None).ToList();
        eventQueue = FindAnyObjectByType<EventQueue>();
        cameraController = FindAnyObjectByType<CameraController>();
    }

    private void ReloadScene()
    {
        gameStateManager.ClearNotSavedKilledEnemies();
        ISet<string> killedEnemies = gameStateManager.GetKilledEnemiesUUids();
        List<ObjectWithHealth> objectsToDelete = new List<ObjectWithHealth>();
        objectsWithHealth
            .Where(obj => killedEnemies.Contains(obj.GetUUid()))
            .ToList()
            .ForEach(objectWithHealth =>
            {
                Destroy(objectWithHealth.gameObject);
                objectsToDelete.Add(objectWithHealth);
            });

        foreach (ObjectWithHealth o in objectsToDelete)
        {
            objectsWithHealth.Remove(o);
        }
        if (gameStateManager.checkpointData != null)
        {
            characterController.transform.position = gameStateManager.checkpointData.position;
            characterController
                .GetComponent<ObjectWithHealth>()
                .healthState.SetHealth(gameStateManager.checkpointData.playerHealth);

            FindObjectsByType<Object>(FindObjectsSortMode.InstanceID)
               .OfType<Restorable>()
               .Where(restorable => gameStateManager.restorableObjects.Contains(restorable.GetBaseObject().GetUUid()))
               .ToList()
               .ForEach(restorable =>
               {
                   restorable.RestoreState();
               });
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        gameOverText.SetActive(false);

        statsToValuesConverter = new StatsToValuesConverter();
        characterController = FindAnyObjectByType<CharacterController>();
        InitializeTraps();
        ReloadScene();
        enemyAttackPositionSearch = GetComponent<EnemyAttackPositionSearch>();
    }

    public void ReloadFromCheckpoint()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void InitializeTraps()
    {
        foreach (
            SpikeTrapSpawner s in FindObjectsByType<SpikeTrapSpawner>(
                FindObjectsSortMode.InstanceID
            )
        )
        {
            s.Initialize();
        }
    }

    private float DistanceBetweenEnemies(ObjectWithHealth enemy, Direction direction)
    {
        return Vector3.Distance(
            characterController.objectToRotateTo.transform.position
                + cameraController.transform.right * (int)direction,
            enemy.transform.position
        );
    }

    private void GetClosestEnemy(Direction direction)
    {
        List<ObjectWithHealth> toLeft = objectsWithHealth
            .Where(obj => obj.GetComponent<Enemy>() != null)
            .Where(
                obj =>
                    Vector3.Distance(obj.transform.position, characterController.transform.position)
                    < minDistanceToFocusOnEnemy
            )
            .Where(obj => obj.gameObject != characterController.objectToRotateTo)
            .Where(obj =>
            {
                float relativePositionX = characterController.transform
                    .InverseTransformPoint(obj.transform.position)
                    .x;
                return direction.Equals(Direction.RIGHT)
                    ? relativePositionX > 0
                    : relativePositionX <= 0;
            })
            .ToList();

        bool invert = false;
        if (toLeft.Count == 0)
        {
            direction = direction.Equals(Direction.LEFT) ? Direction.RIGHT : Direction.LEFT;
            toLeft = objectsWithHealth
                .Where(obj => obj.GetComponent<Enemy>() != null)
                .Where(
                    obj =>
                        Vector3.Distance(
                            obj.transform.position,
                            characterController.transform.position
                        ) < minDistanceToFocusOnEnemy
                )
                .Where(obj =>
                {
                    float relativePositionX = characterController.transform
                        .InverseTransformPoint(obj.transform.position)
                        .x;
                    return direction.Equals(Direction.RIGHT)
                        ? relativePositionX > 0
                        : relativePositionX <= 0;
                })
                .ToList();
            invert = true;
        }
        ObjectWithHealth ob = toLeft.Aggregate(
            (ob1, ob2) =>
                DistanceBetweenEnemies(ob1, direction) < DistanceBetweenEnemies(ob2, direction)
                    ? (invert ? ob2 : ob1)
                    : (invert ? ob1 : ob2)
        );

        characterController.SwitchFocusOnEnemy(ob.gameObject);
    }

    private void ClearInterruptableAnimationsHandler ()
    {
        interruptableAnimationsHandler = null;
    }

    void Update()
    {

        if (ActionKeys.IsKeyPressed(ActionKeys.INTERRUPT_ANIMATION) && interruptableAnimationsHandler != null)
        {
            interruptableAnimationsHandler.InterruptAnimation();
            Invoke(nameof(ClearInterruptableAnimationsHandler), 1f);
        }

        if (
            ActionKeys.IsKeyPressed(ActionKeys.SWITCH_ENEMY_RIGHT)
            && characterController.objectToRotateTo != null
        )
        {
            GetClosestEnemy(Direction.RIGHT);
        }
        if (
            ActionKeys.IsKeyPressed(ActionKeys.SWITCH_ENEMY_LEFT)
            && characterController.objectToRotateTo != null
        )
        {
            GetClosestEnemy(Direction.LEFT);
        }

        if (UnityEngine.Input.GetKeyDown(ActionKeys.RELOAD_SCENE))
        {
            ReloadFromCheckpoint();
        }
        objectsToDelete.Clear();
        if (
            characterController.objectToRotateTo != null
            && PlayersFocusedEnemyIsTooHighOrLow(characterController.objectToRotateTo)
        )
        {
            characterController.ClearFocusedEnemy();
        }
        foreach (ObjectWithHealth objectWithHealth in objectsWithHealth)
        {
            if (!objectWithHealth.gameObject.activeInHierarchy)
            {
                continue;
            }
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
                        characterController.ClearFocusedEnemy();
                    }
                    eventQueue.SubmitEvent(
                        new EventDTO(EventType.ENEMY_KILLED, objectWithHealth.gameObject)
                    );
                    enemyAttackPositionSearch.EnemyGone(objectWithHealth.GetComponent<Enemy>());
                    gameStateManager.AddKilledEnemy(objectWithHealth);
                    if (objectWithHealth.GetComponentInParent<QuestObject>())
                    {
                        objectWithHealth.gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(objectWithHealth.gameObject);
                        objectsToDelete.Add(objectWithHealth);
                    }
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

    private bool PlayersFocusedEnemyIsTooHighOrLow(GameObject playersFocusedEnemy)
    {
        return Mathf.Abs(
                playersFocusedEnemy.transform.position.y - characterController.transform.position.y
            ) > maxVerticalDistanceToFocusOnEnemies;
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
        }
        if (
            Vector3.Distance(characterController.transform.position, enemyObject.transform.position)
                < minDistanceToFocusOnEnemy
            && !PlayersFocusedEnemyIsTooHighOrLow(enemyObject.gameObject)
        )
        {
            characterController.FocusOnEnemy(enemy.gameObject);
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
        }
    }

    internal void SaveCheckpoint(Checkpoint checkpoint)
    {
        int playerHealth = characterController.GetComponent<ObjectWithHealth>().healthState.value;
        gameStateManager.SaveCheckpoint(checkpoint, playerHealth);
    }

    public void SaveCheckpoint(Restorable restorable)
    {
        gameStateManager.AddRestorableObject(restorable);
        Checkpoint checkpoint = FindAnyObjectByType<Checkpoint>();
        Checkpoint newCheckpoint = Instantiate(checkpoint);
        Destroy(newCheckpoint.GetComponent<Collider>());
        newCheckpoint.transform.position =
            FindAnyObjectByType<CharacterController>().transform.position;
        newCheckpoint.SaveCheckpoint();
        Destroy(newCheckpoint);
    }

}
