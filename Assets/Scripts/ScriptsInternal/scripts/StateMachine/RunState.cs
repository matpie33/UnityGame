public class RunState : MovementState
{
    private float runSpeed = 6f;
    private float sprintSpeed = 8f;

    public RunState(CharacterController characterController, PlayerStateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        characterController.rigidbody.isKinematic = false;
        characterController.DisableRootMotionDelayed();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (
            !characterController.canWalkDownLedgeChecker.isColliding
            && ActionKeys.IsKeyPressed(ActionKeys.WALK_DOWN_LEDGE)
        )
        {
            characterController.rigidbody.isKinematic = true;
            characterController.animationsManager.setAnimationToWalkDownLedge();
            stateMachine.ChangeState(stateMachine.doingAnimationState);
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.CROUCH))
        {
            stateMachine.ChangeState(stateMachine.crouchState);
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.DODGE_RIGHT))
        {
            stateMachine.ChangeState(stateMachine.doingAnimationState);
            characterController.animationsManager.SetAnimationToDodgeRight();
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.DODGE_LEFT))
        {
            stateMachine.ChangeState(stateMachine.doingAnimationState);
            characterController.animationsManager.SetAnimationToDodgeLeft();
        }
    }

    public override void OnTrigger(TriggerType triggerType)
    {
        if (triggerType.Equals(TriggerType.MEDIPACK_USED))
        {
            characterController.UseMedipack();
        }
    }

    public override float getTargetSpeed()
    {
        return ActionKeys.IsKeyHold(ActionKeys.SPRINT) ? sprintSpeed : runSpeed;
    }
}
