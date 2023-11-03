using UnityEditor;
using UnityEngine;

public class CrouchState : MovementState
{
    private float crouchSpeed = 3f;

    public CrouchState(CharacterController characterController, StateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        characterController.cameraController.adjustCameraForCrouch();
        characterController.animationsManager.setAnimationToCrouch();
        characterController.changeHeight(false);
    }

    public override void ExitState()
    {
        characterController.cameraController.adjustCameraForStanding();
        characterController.changeHeight(true);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (
            ActionKeys.IsKeyPressed(ActionKeys.CROUCH)
            && !characterController.canStandFromCrouchChecker.isColliding
        )
        {
            stateMachine.ChangeState(stateMachine.runState);
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
        return crouchSpeed;
    }
}
