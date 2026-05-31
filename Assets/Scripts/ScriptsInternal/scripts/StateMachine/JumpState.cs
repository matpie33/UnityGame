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


    override public void FrameUpdate()
    {


        if (characterController.objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HEAD) && characterController.CanGrabLedge() && 
            Vector3.Distance( characterController.objectsInFrontDetector.ledgeCollisionPoint, characterController.transform.position + characterController.capsuleCollider.height * Vector3.up) < 2f)
        {
            characterController.RotatePlayerTowardsWall();
            stateMachine.ChangeState(stateMachine.ledgeGrabState);
            characterController.animationsManager.setAnimationToLedgePrepareHold();
        }
    }

    public override void PhysicsUpdate()
    {
        Vector3 targetVelocity = new(
                characterController.currentVelocity.x,
            characterController.rigidbody.linearVelocity.y,
            characterController.currentVelocity.z
            );
        Vector3 currentVelocity = characterController.rigidbody.linearVelocity;
        Vector3 velocityChange = targetVelocity - currentVelocity;
        characterController.rigidbody.AddForce(
            velocityChange, ForceMode.Impulse
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
                characterController.animationsManager.setAnimationToLandingFromRun();
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
