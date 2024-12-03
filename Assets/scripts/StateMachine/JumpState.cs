using UnityEngine;
using UnityEngine.AI;

public class JumpState : State
{
    private CharacterController characterController;
    private PlayerStateMachine stateMachine;

    public JumpState(CharacterController characterController, PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.characterController = characterController;
    }

    public override void EnterState()
    {
        characterController.rigidbody.AddForce(
            Vector3.up * characterController.jumpForce,
            ForceMode.Impulse
        );
        characterController.UnparentFromRotatingObject();
    }

    public override void FrameUpdate()
    {
        characterController.rigidbody.linearVelocity = new Vector3(
            characterController.currentVelocity.x,
            characterController.rigidbody.linearVelocity.y,
            characterController.currentVelocity.z
        );
        characterController.currentVelocity = Vector3.Lerp(
            characterController.currentVelocity,
            Vector3.zero,
            Time.deltaTime * characterController.horizontalSpeedDecreaseTime
        );

        ObjectsInFrontDetector objectsInFrontDetector = characterController.objectsInFrontDetector;
        if (IsDetectedObjectAWall())
        {
            if (objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HEAD))
            {
                characterController.animationsManager.setAnimationToLedgePrepareHold();
                characterController.SetPlayerPositionToWallHolding();
                stateMachine.ChangeState(stateMachine.ledgeGrabState);
            }
            else if (
                characterController.objectsInFrontDetector.isCollidingWithGround
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
        if (characterController.rigidbody.linearVelocity.y < 0)
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
