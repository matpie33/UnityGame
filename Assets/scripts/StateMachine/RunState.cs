using UnityEditor;
using UnityEngine;

public class RunState : MovementState
{
    private float runSpeed = 6f;

    public RunState(CharacterController characterController, StateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        characterController.animationsManager.setAnimationToMoving();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (ActionKeys.IsKeyPressed(ActionKeys.PUNCH) || ActionKeys.IsKeyPressed(ActionKeys.KICK))
        {
            stateMachine.ChangeState(stateMachine.attackState);
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.SPRINT))
        {
            stateMachine.ChangeState(stateMachine.sprintState);
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.CROUCH))
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
