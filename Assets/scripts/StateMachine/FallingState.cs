using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class FallingState : MovementState
{
    public bool hasReleasedLedge { get; set; }

    public FallingState(CharacterController characterController, PlayerStateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        bool isHittingGround = characterController.groundLandingDetector.IsHittingGround();
        if (isHittingGround)
        {
            stateMachine.ChangeState(stateMachine.runState);
            return;
        }
        if (characterController.currentVelocity.magnitude == 0)
        {
            characterController.animationsManager.setAnimationToFallingFromStanding();
        }
        else
        {
            characterController.animationsManager.setAnimationToFallingFromRunning();
        }
    }

    public override void PhysicsUpdate()
    {
        characterController.rigidbody.AddForce(Vector3.up * -1 * 4, ForceMode.Force);
    }

    public override void FrameUpdate()
    {
        base.Move(characterController.currentVelocity);
        ObjectsInFrontDetector objectsInFrontDetector = characterController.objectsInFrontDetector;
        if (
            !hasReleasedLedge
            && objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HEAD)
            && IsDetectedObjectAWall()
        )
        {
            stateMachine.ChangeState(stateMachine.ledgeGrabState);
        }
    }

    private bool IsDetectedObjectAWall()
    {
        return characterController.objectsInFrontDetector.detectedObject.GetComponent<NavMeshAgent>()
            == null;
    }

    public override void ExitState()
    {
        characterController.animationsManager.setAnimationToMoving();
        hasReleasedLedge = false;
    }

    public override float getTargetSpeed()
    {
        return 3;
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
                break;
            case TriggerType.RELEASED_LEDGE:
                hasReleasedLedge = true;
                break;
        }
    }
}
