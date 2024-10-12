using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class FallingState : MovementState
{
    public bool hasReleasedLedge { get; set; }

    private float verticalSpeed;

    private float fallingHeight;

    public FallingState(CharacterController characterController, PlayerStateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        fallingHeight = characterController.transform.position.y;
        RaycastHit result;
        Physics.Raycast(
            characterController.transform.position,
            Vector3.up * -1,
            out result,
            characterController.minHeightToChangeAnimToFall
        );
        if (result.collider == null)
        {
            if (characterController.currentVelocity.magnitude == 0)
            {
                characterController.animationsManager.setAnimationToFallingFromStanding();
            }
            else
            {
                characterController.animationsManager.setAnimationToFallingFromRunning();
            }
        }
    }

    public override void PhysicsUpdate()
    {
        characterController.rigidbody.AddForce(
            Vector3.up * -1 * characterController.verticalDrag,
            ForceMode.Force
        );
        verticalSpeed = characterController.rigidbody.velocity.y;
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
        float currentPosition = characterController.transform.position.y;
        float currentFallHeight = fallingHeight - currentPosition;
        if (currentFallHeight > characterController.minHeightToChangeAnimToFall) { }
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
                fallingHeight = fallingHeight - characterController.transform.position.y;
                stateMachine.ChangeState(stateMachine.runState);
                if (characterController.currentVelocity.magnitude > 0)
                {
                    characterController.animationsManager.setAnimationToLandingFromRun();
                }
                else
                {
                    characterController.animationsManager.setAnimationToLandingFromStand();
                }
                characterController.modifyHealthAfterLanding(fallingHeight);
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
