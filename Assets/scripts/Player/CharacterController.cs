using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    public PlayerAnimationsManager animationsManager { get; private set; }
    public CapsuleCollider capsuleCollider { get; private set; }
    public CameraController cameraController { get; private set; }
    public PlayerInputs playerInputs { get; private set; }

    public float initialHeight { get; private set; }

    public Rigidbody rigidbody { get; private set; }

    private List<Observer> observers = new List<Observer>();

    public Vector3 currentVelocity { get; set; }

    private HealthState healthState;

    [SerializeField]
    private Image healthBarForeground;

    [SerializeField]
    private TextMeshProUGUI healthText;

    private StateMachine stateMachine;

    [SerializeField]
    private GameObject rightHandObject;

    private PickupObjectsController pickupObjectsController;

    private GameObject pickedObject;

    public PlayerState playerState { get; private set; }

    private UIUpdater uiUpdater;

    private PlayerUI playerUI;

    private LevelData levelData;

    public ObjectsInFrontDetector objectsInFrontDetector { get; private set; }

    public int amountOfStatsToAddPerLevel { get; private set; }

    private StatsAddingDTO statsAddingDTO;

    private void Awake()
    {
        statsAddingDTO = new StatsAddingDTO();
        amountOfStatsToAddPerLevel = 5;
        statsAddingDTO.statsLeft = amountOfStatsToAddPerLevel;
        levelData = new LevelData();
        levelData.level = 1;
        levelData.experience = 0;
        levelData.experienceNeededForNextLevel = 1000;

        objectsInFrontDetector = GetComponent<ObjectsInFrontDetector>();
        playerUI = GetComponent<PlayerUI>();
        cameraController = GetComponent<CameraController>();
        playerInputs = GetComponent<PlayerInputs>();
        rigidbody = GetComponent<Rigidbody>();
        animationsManager = GetComponent<PlayerAnimationsManager>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        pickupObjectsController = GetComponent<PickupObjectsController>();
        uiUpdater = GetComponent<UIUpdater>();
        healthState = new HealthState(200);
        playerState = new PlayerState();
        initialHeight = capsuleCollider.height;
        healthBarForeground.fillAmount = 1;
        uiUpdater = FindObjectOfType<UIUpdater>();
        uiUpdater.InitializeStatsPanel(playerState, playerUI, statsAddingDTO);
        uiUpdater.SetRemainingStatsToAdd(amountOfStatsToAddPerLevel, playerUI);
    }

    public void ResetStats()
    {
        statsAddingDTO.Reset(amountOfStatsToAddPerLevel);
        uiUpdater.UpdateStatsUI(playerState, playerUI);
        uiUpdater.SetRemainingStatsToAdd(amountOfStatsToAddPerLevel, playerUI);
    }

    public void AcceptStats()
    {
        playerState.increaseAgility(statsAddingDTO.agilityIncrease);
        playerState.increaseDefence(statsAddingDTO.defenceIncrease);
        playerState.increaseHealth(statsAddingDTO.healthIncrease);
        playerState.increaseStrength(statsAddingDTO.strengthIncrease);
        statsAddingDTO.Reset(amountOfStatsToAddPerLevel);
        uiUpdater.ToggleVisibilityOfStatsModification(false, playerUI);
        uiUpdater.SetRemainingStatsToAdd(amountOfStatsToAddPerLevel, playerUI);
    }

    public void AddExperience(int value)
    {
        bool isNextLevel = levelData.AddExperience(value);
        if (isNextLevel)
        {
            uiUpdater.ToggleVisibilityOfStatsModification(true, playerUI);
        }
    }

    private void Start()
    {
        stateMachine = GetComponent<StateMachine>();
    }

    public void AttachObjectToHand()
    {
        pickedObject = pickupObjectsController.objectInFront;
        if (pickedObject.GetComponent<Medkit>() != null)
        {
            playerState.increaseMedipacksAmount();
            uiUpdater.UpdateMedipackAmount(
                playerUI.medipackAmountText,
                playerState.numberOfMedipacks
            );
        }
        pickedObject.GetComponent<BoxCollider>().enabled = false;
        pickedObject.transform.SetParent(rightHandObject.transform);
        pickedObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void DestroyPickedObject()
    {
        Destroy(pickedObject);
        pickedObject = null;
    }

    public void DecreaseHealth(int percent)
    {
        healthState.DecreaseHealth(percent);
    }

    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void ClimbingFinished()
    {
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
    }

    public void PickingObjectsStarted()
    {
        stateMachine.OnTriggerType(TriggerType.PICKUP_STARTED);
    }

    private void Update()
    {
        if (playerState.HasMedipacks() && ActionKeys.IsKeyPressed(ActionKeys.USE_MEDIPACK))
        {
            stateMachine.OnTriggerType(TriggerType.MEDIPACK_USED);
        }
        uiUpdater.UpdateHealthBar(healthState, playerUI.healthText, playerUI.healthBar);
        uiUpdater.UpdateExperience(
            levelData,
            playerUI.experienceText,
            playerUI.experienceBar,
            playerUI.levelText
        );
    }

    public void UseMedipack()
    {
        if (healthState.value == healthState.maxHealth)
        {
            return;
        }
        healthState.IncreaseHealth(30);
        playerState.decreaseMedipacksAmount();
        uiUpdater.UpdateMedipackAmount(playerUI.medipackAmountText, playerState.numberOfMedipacks);
    }

    public bool IsAttacking()
    {
        return playerState.isAttacking;
    }

    public void attackEventChecked()
    {
        playerState.isAttacking = false;
    }

    public void attackAnimationFinish()
    {
        playerState.isAttacking = false;
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
    }

    public void attackAnimationStart()
    {
        playerState.isAttacking = true;
    }

    private void notifyObservers(bool isAttacking)
    {
        EventDTO eventDTO = EventDTO.createEventPlayerAttack(isAttacking);
        foreach (Observer observer in observers)
        {
            observer.Notify(eventDTO);
        }
    }

    public void GroundDetected()
    {
        stateMachine.OnTriggerType(TriggerType.GROUND_DETECTED);
    }

    public void changeHeight(bool toStanding)
    {
        if (toStanding)
        {
            capsuleCollider.height = initialHeight;
            capsuleCollider.center = new Vector3(0, 0.9f, 0);
        }
        else
        {
            capsuleCollider.height = initialHeight / 2;
            capsuleCollider.center = new Vector3(0, 0.47f, 0);
        }
    }

    public void ledgeDetected()
    {
        stateMachine.OnTriggerType(TriggerType.LEDGE_DETECTED);
    }

    internal void pickupAnimationFinished()
    {
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
    }
}
