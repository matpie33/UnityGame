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
        Physics.gravity = new Vector3(0, -10.0F, 0);

        currentState = runState;
        currentState.EnterState();
    }

    private void FixedUpdate()
    {
        if (
            characterController.rigidbody.linearVelocity.y < -0.5
            && !characterController.objectsInFrontDetector.isCollidingWithGround
            && currentState != doingAnimationState
        )
        {
            ChangeState(fallingState);
            Vector3 velocity = characterController.currentVelocity;
            if (velocity.x != 0 || velocity.z != 0)
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
        if (characterController.debugPlayerStates)
        {
            Debug.Log(currentState);
        }
    }
}
