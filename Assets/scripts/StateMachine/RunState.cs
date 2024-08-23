using UnityEditor;
using UnityEngine;

public class RunState : MovementState
{
    private float runSpeed = 6f;

    public RunState(CharacterController characterController, PlayerStateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        characterController.rigidbody.isKinematic = false;
        characterController.animationsManager.disableRootMotion();
        characterController.animationsManager.setAnimationToMoving();
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
        else if (ActionKeys.IsKeyPressed(ActionKeys.SPRINT))
        {
            stateMachine.ChangeState(stateMachine.sprintState);
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.CROUCH))
        {
            stateMachine.ChangeState(stateMachine.crouchState);
        }
    }

    public override void OnTrigger(TriggerType triggerType)
    {
        if (triggerType.Equals(TriggerType.GROUND_DETECTED))
        {
            characterController.animationsManager.setAnimationToMoving();
            stateMachine.ChangeState(stateMachine.runState);
        }
        else if (triggerType.Equals(TriggerType.MEDIPACK_USED))
        {
            characterController.UseMedipack();
        }
    }

    public override float getTargetSpeed()
    {
        return runSpeed;
    }
}
