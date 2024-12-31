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
        ObjectsInFrontDetector objectsInFrontDetector = characterController.objectsInFrontDetector;
        if (IsDetectedObjectAWall())
        {
            if (objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HEAD))
            {
                characterController.animationsManager.setAnimationToLedgePrepareHold();
                stateMachine.ChangeState(stateMachine.ledgeGrabState);
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
    }

    public override void OnTrigger(TriggerType triggerType)
    {
        switch (triggerType)
        {
            case TriggerType.GROUND_DETECTED:
                stateMachine.ChangeState(stateMachine.runState);
                break;
            case TriggerType.PLAYER_COLLIDED:
                characterController.rigidbody.linearVelocity = Vector3.up * -1;
                characterController.currentVelocity = Vector3.up * -1 * Time.deltaTime;
                stateMachine.ChangeState(stateMachine.fallingState);
                break;
        }
    }
}
