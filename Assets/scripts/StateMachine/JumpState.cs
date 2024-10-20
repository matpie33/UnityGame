using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class JumpState : MovementState
{
    public JumpState(CharacterController characterController, PlayerStateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        characterController.rigidbody.AddForce(
            Vector3.up * characterController.jumpForce,
            ForceMode.Impulse
        );
    }

    public override float getTargetSpeed()
    {
        return 3f;
    }

    public override void FrameUpdate()
    {
        base.Move(characterController.currentVelocity);
        ObjectsInFrontDetector objectsInFrontDetector = characterController.objectsInFrontDetector;
        if (IsDetectedObjectAWall())
        {
            if (objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HEAD))
            {
                stateMachine.ChangeState(stateMachine.ledgeGrabState);
            }
            else if (
                characterController.groundLandingDetector.IsHittingGround()
                && objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HIPS)
            )
            {
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                characterController.animationsManager.PlayMiddleWallClimb();
            }
        }
    }

    private bool IsDetectedObjectAWall()
    {
        return !characterController.objectsInFrontDetector.detectedWallType.Equals(WallType.NO_WALL)
            && characterController.objectsInFrontDetector.detectedObject.GetComponent<NavMeshAgent>()
                == null;
    }

    public override void PhysicsUpdate()
    {
        if (characterController.rigidbody.linearVelocity.y < -0.5)
        {
            stateMachine.ChangeState(stateMachine.fallingState);
            return;
        }
    }

    public override void OnTrigger(TriggerType triggerType)
    {
        switch (triggerType)
        {
            case TriggerType.GROUND_DETECTED:
                stateMachine.ChangeState(stateMachine.runState);
                break;
            case TriggerType.PLAYER_COLLIDED:
                characterController.currentVelocity = Vector3.up * -1 * Time.deltaTime;
                stateMachine.ChangeState(stateMachine.fallingState);
                break;
        }
    }
}
