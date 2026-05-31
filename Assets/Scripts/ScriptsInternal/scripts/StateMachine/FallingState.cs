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

        if (releasedLedge != characterController.objectsInFrontDetector.detectedObject 
            && characterController.objectsInFrontDetector.detectedObject != null && 
            characterController.objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HEAD) && characterController.CanGrabLedge() &&
            Vector3.Distance(characterController.objectsInFrontDetector.ledgeCollisionPoint, characterController.transform.position + characterController.capsuleCollider.height * Vector3.up) < 0.6f)
        {
            characterController.RotatePlayerTowardsWall();
            stateMachine.ChangeState(stateMachine.ledgeGrabState);
            characterController.animationsManager.setAnimationToLedgePrepareHold();
        }
    }

    public override void PhysicsUpdate()
    {
        base.Move(characterController.currentVelocity.normalized, characterController.transform.forward);
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
            case TriggerType.GROUND_DETECTED:
                stateMachine.ChangeState(stateMachine.runState);
                Vector3 horizontalVelocity = new Vector3(characterController.currentVelocity.x, 0, characterController.currentVelocity.z);
                if (horizontalVelocity.magnitude > 0.01f)
                {
                    characterController.animationsManager.setAnimationToLandingFromRun();
                }
                else
                {
                    characterController.animationsManager.setAnimationToLandingFromStand();
                }
                break;
        }
    }
}
