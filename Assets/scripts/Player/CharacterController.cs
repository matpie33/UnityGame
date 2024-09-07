using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController : Observer
{
    public WallData wallData { get; private set; }
    public PlayerAnimationsManager animationsManager { get; private set; }
    public CapsuleCollider capsuleCollider { get; private set; }
    public CameraController cameraController { get; private set; }

    [field: SerializeField]
    public float minHeightToChangeAnimToFall;

    [SerializeField]
    public float forwardOffset;

    [SerializeField]
    public float upOffset;

    [field: SerializeField]
    public float verticalDrag { get; private set; }

    [field: SerializeField]
    public float jumpForce { get; private set; }

    public float initialHeight { get; private set; }

    public new Rigidbody rigidbody { get; private set; }

    public Vector3 currentVelocity { get; set; }

    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerState playerState { get; private set; }

    private UIUpdater uiUpdater;

    public LevelData levelData { get; private set; }

    public ObjectsInFrontDetector objectsInFrontDetector { get; private set; }

    [field: SerializeField]
    public TriggerDetector canClimbUpWallChecker { get; private set; }

    [field: SerializeField]
    public TriggerDetector canStandFromCrouchChecker { get; private set; }

    [field: SerializeField]
    public TriggerDetector canWalkDownLedgeChecker { get; private set; }

    public LedgeContinuationDetector ledgeContinuationDetector { get; private set; }

    public GroundLandingDetector groundLandingDetector { get; private set; }

    public PlayerBackpack playerBackpack { get; private set; }

    private EventQueue eventQueue;

    [SerializeField]
    private Quest quest;

    private ObjectWithHealth objectWithHealth;

    [SerializeField]
    private GameObject legStepUpTarget;

    [SerializeField]
    private GameObject hipStepUpTarget;

    [SerializeField]
    private GameObject hips;

    [SerializeField]
    private int minHeightToDecreaseHp;

    [SerializeField]
    private int hpDecrease;

    private void Awake()
    {
        wallData = new WallData();
        playerBackpack = new PlayerBackpack();
        levelData = new LevelData();

        eventQueue = FindAnyObjectByType<EventQueue>();
        objectsInFrontDetector = GetComponent<ObjectsInFrontDetector>();

        cameraController = GetComponent<CameraController>();
        rigidbody = GetComponent<Rigidbody>();
        animationsManager = new PlayerAnimationsManager(GetComponent<Animator>());
        capsuleCollider = GetComponent<CapsuleCollider>();
        ledgeContinuationDetector = GetComponent<LedgeContinuationDetector>();
        groundLandingDetector = GetComponentInChildren<GroundLandingDetector>();

        playerState = new PlayerState();
        initialHeight = capsuleCollider.height;

        uiUpdater = FindAnyObjectByType<UIUpdater>();
        objectWithHealth = GetComponent<ObjectWithHealth>();
    }

    private void Start()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        uiUpdater.InitializeStatsPanel(GetStats());
        uiUpdater.UpdatePlayerHealth(objectWithHealth.healthState);
    }

    public Stats GetStats()
    {
        return objectWithHealth.stats;
    }

    public void GetWallData()
    {
        wallData.wallCollider = objectsInFrontDetector.wallCollider;
        wallData.directionFromPlayerToWall = objectsInFrontDetector.directionFromPlayerToWall;
        wallData.verticalCollisionPoint = objectsInFrontDetector.verticalCollisionPosition;
        wallData.horizontalCollisionPoint = objectsInFrontDetector.horizontalCollisionPosition;
    }

    public void TryShimmy(LedgeDirection direction)
    {
        stateMachine.shimmyState.Direction(direction);
        stateMachine.ChangeState(stateMachine.shimmyState);
    }

    public void AddExperience(int value)
    {
        bool isNextLevel = levelData.AddExperience(value);
        if (isNextLevel)
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.CHARACTER_LEVEL_UP, null));
            uiUpdater.SetVisibilityOfStatsModification(true);
        }
    }

    public void ClimbingFinished()
    {
        transform.position = wallData.verticalCollisionPoint;
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
    }

    public void UseMedipack()
    {
        HealthState healthState = GetComponent<ObjectWithHealth>().healthState;
        if (healthState.value == healthState.maxHealth)
        {
            return;
        }
        healthState.IncreaseHealth(30);
        playerState.decreaseMedipacksAmount();
        uiUpdater.UpdateMedipackAmount(playerState.numberOfMedipacks);
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
            capsuleCollider.height = 1.4f;
            capsuleCollider.center = new Vector3(0, 0.6f, 0);
        }
    }

    internal void pickupAnimationFinished()
    {
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.OBJECT_NOW_IN_RANGE:
                GameObject eventData = (GameObject)eventDTO.eventData;
                if (eventData.GetComponent<Interactable>() != null)
                {
                    Interactable interactable = eventData.GetComponent<Interactable>();
                    playerState.objectToInteractWith = interactable;
                }
                break;
            case EventType.OBJECT_OUT_OF_RANGE:
                if (!playerState.isPickingObject)
                {
                    playerState.objectToInteractWith = null;
                }
                break;
            case EventType.GATE_OPENED:
                animationsManager.setAnimationToMoving();
                break;
            case EventType.QUEST_ACCEPTED:
            case EventType.QUEST_REJECTED:
                stateMachine.ChangeState(stateMachine.runState);
                break;
            case EventType.PLAYER_TALKING_TO_NPC:
                animationsManager.setRunningSpeedParameter(0);
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                break;
            case EventType.PLAYER_COLLIDED:
                stateMachine.OnTriggerType(TriggerType.PLAYER_COLLIDED);
                break;
        }
    }

    internal void ShimmyDone()
    {
        stateMachine.OnTriggerType(TriggerType.SHIMMY_DONE);
    }

    internal void ShimmyContinue(LedgeDirection ledgeDirection)
    {
        stateMachine.shimmyState.ShimmyContinue(ledgeDirection);
    }

    internal void IncreaseMaxHealth(int healthIncrease)
    {
        GetComponent<ObjectWithHealth>().healthState.IncreaseMaxHealth(healthIncrease);
    }

    internal void SetLegAndHipTarget(Vector3 verticalCollisionPoint)
    {
        legStepUpTarget.transform.position = verticalCollisionPoint - transform.forward * 0.3f;
        Vector3 distanceBetweenHipsAndCollisionPoint =
            hips.transform.position - verticalCollisionPoint;
        distanceBetweenHipsAndCollisionPoint.y = 0;
        hipStepUpTarget.transform.position =
            verticalCollisionPoint
            + Vector3.up * 1f
            + distanceBetweenHipsAndCollisionPoint
            + transform.forward * 0.2f;
    }

    public void modifyHealthAfterLanding(float fallingHeight)
    {
        float difference = fallingHeight - minHeightToDecreaseHp;
        if (difference > 0)
        {
            int healthDecrease = (int)Math.Round(difference * hpDecrease);

            objectWithHealth.DecreaseHealth(healthDecrease);
        }
    }
}
