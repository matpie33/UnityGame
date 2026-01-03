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
        characterController.UnparentFromRotatingObject();
    }

    public override void FrameUpdate()
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

    public override void PhysicsUpdate()
    {
        base.Move(characterController.currentVelocity.normalized, characterController.transform.forward);
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

            case TriggerType.PLAYER_COLLIDED:
                characterController.currentVelocity = Vector3.up * -1 * Time.deltaTime;
                break;
        }
    }
}
