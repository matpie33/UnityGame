using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public RunState runState { get; private set; }
    public JumpState jumpState { get; private set; }
    public CrouchState crouchState { get; private set; }
    public LedgeGrabState ledgeGrabState { get; private set; }
    public FallingState fallingState { get; private set; }
    public ShimmyState shimmyState { get; private set; }
    public DoingAnimationState doingAnimationState { get; private set; }

    private CharacterController characterController;

    [SerializeField]
    public bool debugPlayerStates;

    public float fallingStartingPositionY { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        runState = new RunState(characterController, this);
        ledgeGrabState = new LedgeGrabState(characterController, this);
        crouchState = new CrouchState(characterController, this);
        jumpState = new JumpState(characterController, this);
        fallingState = new FallingState(characterController, this);
        shimmyState = new ShimmyState(characterController, this);
        doingAnimationState = new DoingAnimationState();

        currentState = runState;
        currentState.EnterState();
    }

    private void FixedUpdate()
    {
        if (
            !characterController.objectsInFrontDetector.isCollidingWithGround
            && currentState != doingAnimationState
            && characterController.rigidbody.linearVelocity.y < -1f
        )
        {
            ChangeState(fallingState);
            Vector3 velocity = characterController.currentVelocity;
            if (Mathf.Abs(velocity.x) > 0.01f || (Mathf.Abs(velocity.z) > 0.01f))
            {
                characterController.animationsManager.setAnimationToFallingFromRunning();
            }
            else
            {
                characterController.animationsManager.setAnimationToFallingFromStanding();
            }
        }
        currentState.PhysicsUpdate();
    }

    public void OnTriggerType(TriggerType triggerType)
    {
        switch (triggerType)
        {
            case TriggerType.ENTER_LEDGE_GRAB_STATE:
                ChangeState(ledgeGrabState);
                break;

            case TriggerType.MEDIPACK_USED:
            case TriggerType.GROUND_DETECTED:
            case TriggerType.PLAYER_COLLIDED:
            case TriggerType.RELEASED_LEDGE:
                currentState.OnTrigger(triggerType);
                break;

            case TriggerType.ANIMATION_FINISHED:
            case TriggerType.CLIMBING_FINISHED:
                ChangeState(runState);
                break;
        }
    }

    private void Update()
    {
        base.BaseUpdate();
        if (debugPlayerStates)
        {
            Debug.Log(currentState);
        }
    }

    internal void StartedFalling()
    {
        fallingStartingPositionY = transform.position.y;
    }
}
