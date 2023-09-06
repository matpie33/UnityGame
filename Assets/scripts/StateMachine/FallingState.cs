using UnityEditor;
using UnityEngine;

public class FallingState : MovementState
{
    public FallingState(CharacterController characterController, StateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        characterController.animationsManager.setAnimationToFalling();
    }

    public override void PhysicsUpdate()
    {
        characterController.rigidbody.AddForce(Vector3.up * -1 * 4, ForceMode.Force);
        if (characterController.rigidbody.velocity.y > -0.5)
        {
            stateMachine.ChangeState(characterController.runState);
        }
    }

    public override void FrameUpdate()
    {
        Debug.Log(characterController.currentVelocity);
        base.Move(characterController.currentVelocity);
    }

    public override void ExitState()
    {
        characterController.animationsManager.setAnimationToGrounded();
    }

    public override float getTargetSpeed()
    {
        return 3;
    }
}
