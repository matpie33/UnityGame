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
        if (UnityEngine.Input.GetKeyDown(KeyCode.P) || UnityEngine.Input.GetKeyDown(KeyCode.K))
        {
            stateMachine.ChangeState(characterController.attackState);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
        {
            stateMachine.ChangeState(characterController.sprintState);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftControl))
        {
            stateMachine.ChangeState(characterController.crouchState);
        }
    }

    public override void OnTrigger(TriggerType triggerType)
    {
        if (triggerType.Equals(TriggerType.GROUND_DETECTED))
        {
            characterController.animationsManager.setAnimationToMoving();
            stateMachine.ChangeState(characterController.runState);
        }
    }

    public override float getTargetSpeed()
    {
        return runSpeed;
    }
}
