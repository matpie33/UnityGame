using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class FallingState : MovementState
{
    private float fallingHeight;

    public GameObject releasedLedge { get; set; }

    public FallingState(CharacterController characterController, PlayerStateMachine stateMachine)
        : base(characterController, stateMachine) { }

    public override void EnterState()
    {
        fallingHeight = characterController.transform.position.y;
        releasedLedge = null;
        playerMoving = true;
        characterController.UnparentFromRotatingObject();
    }

    public override void PhysicsUpdate()
    {
        ObjectsInFrontDetector objectsInFrontDetector = characterController.objectsInFrontDetector;
        if (
            objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HEAD)
            && IsDetectedObjectAWall()
            && releasedLedge != characterController.objectsInFrontDetector.detectedObject
        )
        {
            characterController.animationsManager.setAnimationToLedgePrepareHold();
            stateMachine.ChangeState(stateMachine.ledgeGrabState);
        }
    }

    public override void FrameUpdate()
    {
        base.Move(characterController.currentVelocity.normalized);
    }

    private bool IsDetectedObjectAWall()
    {
        return characterController.objectsInFrontDetector.detectedObject != null
            && characterController.objectsInFrontDetector.detectedObject.GetComponent<NavMeshAgent>()
                == null;
    }

    public override float getTargetSpeed()
    {
        return 0;
    }

    public override void OnTrigger(TriggerType triggerType)
    {
        switch (triggerType)
        {
            case TriggerType.GROUND_DETECTED:
                fallingHeight =
                    characterController.stateMachine.fallingStartingPositionY
                    - characterController.transform.position.y;
                stateMachine.ChangeState(stateMachine.runState);
                Vector3 velo = characterController.currentVelocity;
                Vector3 forwardVelocity = new Vector3(velo.x, 0, velo.z);
                if (forwardVelocity.magnitude > 0.01f)
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
        }
    }
}
