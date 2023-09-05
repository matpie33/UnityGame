using UnityEditor;
using UnityEngine;

public class CrouchState : MovementState
{
    private float crouchSpeed = 3f;

    public CrouchState(CharacterController characterController, StateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        characterController.animationsManager.setAnimationToCrouch();
        characterController.capsuleCollider.height = characterController.initialHeight / 2;
        characterController.capsuleCollider.center = new Vector3(0, 0.47f, 0);
    }

    public override void ExitState()
    {
        characterController.capsuleCollider.height = characterController.initialHeight;
        characterController.capsuleCollider.center = new Vector3(0, 0.9f, 0);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftControl))
        {
            stateMachine.ChangeState(characterController.runState);
        }
    }

    public override float getTargetSpeed()
    {
        return crouchSpeed;
    }
}
