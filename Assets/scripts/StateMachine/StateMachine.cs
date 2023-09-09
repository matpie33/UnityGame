using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private State currentState;
    public RunState runState { get; private set; }
    public JumpState jumpState { get; private set; }
    public SprintState sprintState { get; private set; }
    public CrouchState crouchState { get; private set; }
    public LedgeGrabState ledgeGrabState { get; private set; }
    public AttackState attackState { get; private set; }
    public FallingState fallingState { get; private set; }
    public PickupObjectsState pickupObjectsState { get; private set; }

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        runState = new RunState(characterController, this);
        ledgeGrabState = new LedgeGrabState(characterController, this);
        sprintState = new SprintState(characterController, this);
        crouchState = new CrouchState(characterController, this);
        jumpState = new JumpState(characterController, this);
        attackState = new AttackState(characterController);
        fallingState = new FallingState(characterController, this);
        pickupObjectsState = new PickupObjectsState(characterController);

        currentState = runState;
        currentState.EnterState();
    }

    public void ChangeState(State state)
    {
        if (currentState == state)
        {
            return;
        }
        currentState.ExitState();
        currentState = state;
        state.EnterState();
    }

    private void FixedUpdate()
    {
        if (characterController.rigidbody.velocity.y < -0.5)
        {
            ChangeState(fallingState);
        }
        currentState.PhysicsUpdate();
    }

    public void OnTriggerType(TriggerType triggerType)
    {
        switch (triggerType)
        {
            case TriggerType.LEDGE_DETECTED:
            case TriggerType.MEDIPACK_USED:
            case TriggerType.GROUND_DETECTED:
                currentState.OnTrigger(triggerType);
                break;

            case TriggerType.ANIMATION_FINISHED:
            case TriggerType.CLIMBING_FINISHED:
                ChangeState(runState);
                break;
            case TriggerType.PICKUP_STARTED:
                ChangeState(pickupObjectsState);
                break;
        }
    }

    private void Update()
    {
        currentState.FrameUpdate();
    }
}
