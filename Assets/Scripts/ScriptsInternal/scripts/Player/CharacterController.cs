using System;
using UnityEngine;

public class CharacterController : Observer
{
    public bool IsPlayerParented = false;

    [SerializeField]
    private GameObject destination;

    [SerializeField]
    public float horizontalSpeedDecreaseTime;

    private int collisionCount;

    public WallData wallData { get; private set; }
    public PlayerAnimationsManager animationsManager { get; private set; }
    public CapsuleCollider capsuleCollider { get; private set; }
    public CameraController cameraController { get; private set; }

    public float currentWallHeight { get; set; }

    [field: SerializeField]
    public float minHeightToChangeAnimToFall;

    [SerializeField]
    public float forwardOffset;

    [SerializeField]
    public float upOffset;

    [SerializeField]
    public float maxSlope;

    [field: SerializeField]
    public float jumpForce { get; private set; }

    public float initialHeight { get; private set; }

    private Vector3 initialColliderCenter;

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

    public PlayerBackpack playerBackpack { get; private set; }

    public EventQueue eventQueue { get; private set; }

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
    private int hpDecreaseFromFallingPerOneUnit;

    [SerializeField]
    private Vector3 offsetForLedgeGrabbing;

    public ObstacleDetector obstacleDetector { get; private set; }

    [field: SerializeField]
    public float moveSharpness { get; private set; }

    [field: SerializeField]
    public float rotationSharpness { get; private set; }

    public float playerColliderRadius { get; private set; }

    private float fallingStartingHeight;

    public GroundDetector groundDetector { get; private set; }

    private void Awake()
    {
        wallData = new WallData();
        playerBackpack = new PlayerBackpack();
        levelData = new LevelData();

        obstacleDetector = GetComponentInChildren<ObstacleDetector>();
        eventQueue = FindAnyObjectByType<EventQueue>();
        objectsInFrontDetector = GetComponent<ObjectsInFrontDetector>();

        cameraController = FindAnyObjectByType<CameraController>();
        rigidbody = GetComponent<Rigidbody>();
        animationsManager = new PlayerAnimationsManager(GetComponent<Animator>());
        capsuleCollider = GetComponent<CapsuleCollider>();
        ledgeContinuationDetector = GetComponent<LedgeContinuationDetector>();

        playerState = new PlayerState();
        playerColliderRadius = capsuleCollider.radius;
        initialHeight = capsuleCollider.height;
        initialColliderCenter = capsuleCollider.center;

        uiUpdater = FindAnyObjectByType<UIUpdater>();
        objectWithHealth = GetComponent<ObjectWithHealth>();
    }

    private void Start()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        groundDetector = FindAnyObjectByType<GroundDetector>();
        uiUpdater.InitializeStatsPanel(GetStats());
        uiUpdater.UpdatePlayerHealth(objectWithHealth.healthState);
        Physics.gravity = new Vector3(0, -20.0F, 0);
    }

    public bool IsColliding()
    {
        return collisionCount > 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rigidbody.isKinematic = false;
        eventQueue.SubmitEvent(new EventDTO(EventType.PLAYER_COLLIDED, null));
        collisionCount++;
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionCount--;
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
        capsuleCollider.enabled = true;
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

    public void attackAnimationStart()
    {
        playerState.isAttacking = true;
    }

    public void changeHeight(bool toStanding)
    {
        if (toStanding)
        {
            capsuleCollider.height = initialHeight;
            capsuleCollider.center = initialColliderCenter;
        }
        else
        {
            capsuleCollider.height = 1.3f;
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
            case EventType.STARTED_FALLING:
                fallingStartingHeight = transform.position.y;
                break;
            case EventType.OBJECT_NOW_IN_RANGE:
                GameObject eventData = (GameObject)eventDTO.eventData;
                if (eventData.GetComponent<Interactable>() != null)
                {
                    Interactable interactable = eventData.GetComponent<Interactable>();
                    playerState.objectToInteractWith = interactable;
                }
                break;
            case EventType.OBJECT_OUT_OF_RANGE:
                GameObject gameObject = (GameObject)eventDTO.eventData;
                if (!playerState.isPickingObject && gameObject.GetComponent<Interactable>() != null)
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
            case EventType.GROUND_DETECTED:
                stateMachine.OnTriggerType(TriggerType.GROUND_DETECTED);
                GameObject ground = (GameObject)eventDTO.eventData;
                ParentToRotatingObject(groundDetector.ground);
                UnparentIfNotRotatingObject(ground);
                float fallingHeight = HandleGrounding();
                modifyHealthAfterLanding(fallingHeight);
                break;
        }
    }


    private float HandleGrounding()
    {
        float fallingHeight =
                            fallingStartingHeight
                            - transform.position.y;
        

        return fallingHeight;
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
            int healthDecrease = (int)Math.Round(difference * hpDecreaseFromFallingPerOneUnit);

            objectWithHealth.DecreaseHealth(healthDecrease);
        }
    }

    public void RotatePlayerTowardsWall()
    {
        GetWallData();

        transform.rotation = Quaternion.LookRotation(
            wallData.directionFromPlayerToWall,
            Vector3.up
        );
    }

    public void ParentToRotatingObject(GameObject objectToCheck)
    {
        if (
            !IsPlayerParented
            && Utils.DoesParentHaveComponent(
                objectToCheck,
                typeof(RotatingObject)
            )
        )
        {
            transform.parent = groundDetector.ground.transform.parent;
            IsPlayerParented = true;
        }
    }

    public void UnparentIfNotRotatingObject(GameObject ground)
    {
        if (
            IsPlayerParented
            && !Utils.DoesParentHaveComponent(
                ground,
                typeof(RotatingObject)
            )
        )
        {
            transform.parent = null;
            IsPlayerParented = false;
        }
    }

    public void UnparentFromRotatingObject()
    {
        if (IsPlayerParented)
        {
            transform.parent = null;
            IsPlayerParented = false;
        }
    }

    private void DisableRootMotion()
    {
        animationsManager.DisableRootMotion();
    }

    internal void DisableRootMotionDelayed()
    {
        Invoke(nameof(DisableRootMotion), 0.5f);
    }
}
