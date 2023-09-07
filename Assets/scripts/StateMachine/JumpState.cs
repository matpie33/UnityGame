using UnityEditor;
using UnityEngine;

public class JumpState : MovementState
{
    public float jumpForce = 10;
    public float gravity = 4;

    public JumpState(CharacterController characterController, StateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        characterController.rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        characterController.animationsManager.setAnimationToJumping();
    }

    public override float getTargetSpeed()
    {
        return 3f;
    }

    public override void FrameUpdate()
    {
        base.Move(characterController.currentVelocity);
    }

    public override void PhysicsUpdate()
    {
        characterController.rigidbody.AddForce(Vector3.up * -1 * gravity, ForceMode.Force);
        if (characterController.rigidbody.velocity.y < -0.5)
        {
            stateMachine.ChangeState(characterController.fallingState);
        }
    }

    public override void OnTrigger(TriggerType triggerType)
    {
        if (triggerType.Equals(TriggerType.LEDGE_DETECTED))
        {
            stateMachine.ChangeState(characterController.ledgeGrabState);
        }
        if (triggerType.Equals(TriggerType.GROUND_DETECTED))
        {
            characterController.animationsManager.setAnimationToMoving();
            stateMachine.ChangeState(characterController.runState);
        }
    }
}
