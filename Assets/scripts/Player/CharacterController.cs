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
    public PlayerInputs playerInputs { get; private set; }

    public float initialHeight { get; private set; }

    public new Rigidbody rigidbody { get; private set; }

    public Vector3 currentVelocity { get; set; }

    public HealthState healthState { get; private set; }

    [SerializeField]
    private Image healthBarForeground;

    [SerializeField]
    private TextMeshProUGUI healthText;

    public StateMachine stateMachine { get; private set; }

    [SerializeField]
    private GameObject rightHandObject;

    public PlayerState playerState { get; private set; }

    private UIUpdater uiUpdater;

    private LevelData levelData;

    public ObjectsInFrontDetector objectsInFrontDetector { get; private set; }

    private EventQueue eventQueue;

    [field: SerializeField]
    public TriggerDetector canClimbUpWallChecker { get; private set; }

    [field: SerializeField]
    public TriggerDetector canStandFromCrouchChecker { get; private set; }

    [field: SerializeField]
    public TriggerDetector canWalkDownLedgeChecker { get; private set; }

    public LedgeContinuationDetector ledgeContinuationDetector { get; private set; }

    public GroundLandingDetector groundLandingDetector { get; private set; }

    public PlayerBackpack playerBackpack { get; private set; }

    [SerializeField]
    private GameObject rightHandTarget;

    private void Awake()
    {
        wallData = new WallData();
        playerBackpack = new PlayerBackpack();
        levelData = new LevelData();
        levelData.level = 1;
        levelData.experience = 0;
        levelData.experienceNeededForNextLevel = 1000;

        eventQueue = FindObjectOfType<EventQueue>();
        objectsInFrontDetector = GetComponent<ObjectsInFrontDetector>();

        cameraController = GetComponent<CameraController>();
        playerInputs = GetComponent<PlayerInputs>();
        rigidbody = GetComponent<Rigidbody>();
        animationsManager = GetComponent<PlayerAnimationsManager>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        ledgeContinuationDetector = GetComponent<LedgeContinuationDetector>();
        groundLandingDetector = GetComponentInChildren<GroundLandingDetector>();

        healthState = new HealthState(200);
        playerState = new PlayerState();
        initialHeight = capsuleCollider.height;
        healthBarForeground.fillAmount = 1;
        uiUpdater = FindObjectOfType<UIUpdater>();
    }

    private void Start()
    {
        stateMachine = GetComponent<StateMachine>();
        uiUpdater.InitializeStatsPanel(playerState);
    }

    public void GetWallData()
    {
        wallData.wallCollider = objectsInFrontDetector.wallCollider;
        wallData.directionFromPlayerToWall = objectsInFrontDetector.directionFromPlayerToWall;
        wallData.distanceToCollider = objectsInFrontDetector.distanceToCollider;
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
            uiUpdater.ToggleVisibilityOfStatsModification(true);
        }
    }

    public void DecreaseHealth(int percent)
    {
        healthState.DecreaseHealth(percent);
    }

    public void ClimbingFinished()
    {
        stateMachine.OnTriggerType(TriggerType.ANIMATION_FINISHED);
    }

    private void Update()
    {
        if (playerState.HasMedipacks() && ActionKeys.IsKeyPressed(ActionKeys.USE_MEDIPACK))
        {
            stateMachine.OnTriggerType(TriggerType.MEDIPACK_USED);
        }
        if (
            playerState.objectToInteractWith != null && ActionKeys.IsKeyPressed(ActionKeys.INTERACT)
        )
        {
            Interactable objectToInteractWith = playerState.objectToInteractWith;
            if (objectToInteractWith.GetType() == typeof(Lever))
            {
                animationsManager.setAnimationToPullLever();
                eventQueue.SubmitEvent(
                    new EventDTO(EventType.LEVER_OPENING, objectToInteractWith.gameObject)
                );
            }
            else if (objectToInteractWith.GetType() == typeof(Pickable))
            {
                animationsManager.setAnimationToPickup();
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                playerState.isPickingObject = true;
                rightHandTarget.transform.position = objectToInteractWith.transform.position;
            }
            else if (objectToInteractWith.GetType() == typeof(LockedDoor))
            {
                LockedDoor door = (LockedDoor)playerState.objectToInteractWith;
                if (!door.PlayerHasKey())
                {
                    return;
                }
                animationsManager.SetAnimationToOpenDoor();
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                rightHandTarget.transform.position = door.lockTransform.position;

                door.isOpened = true;
            }
            eventQueue.SubmitEvent(new EventDTO(EventType.INTERACTION_DONE, null));
        }
        uiUpdater.UpdatePlayerHealth(healthState);
        uiUpdater.UpdateExperience(levelData);
    }

    public void UseMedipack()
    {
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

    public void changeStateToLedgeGrab()
    {
        stateMachine.ChangeState(stateMachine.ledgeGrabState);
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
            capsuleCollider.height = initialHeight / 2;
            capsuleCollider.center = new Vector3(0, 0.47f, 0);
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
}
