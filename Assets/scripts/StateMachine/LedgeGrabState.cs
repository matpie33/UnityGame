using UnityEditor;
using UnityEngine;

public class LedgeGrabState : State
{
    private CharacterController characterController;
    private StateMachine stateMachine;

    public LedgeGrabState(CharacterController characterController, StateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        characterController.rigidbody.isKinematic = true;
        characterController.animationsManager.setAnimationToLedgeGrab();
    }

    public override void ExitState()
    {
        characterController.animationsManager.disableRootMotion();
        characterController.capsuleCollider.height = characterController.initialHeight;
        characterController.capsuleCollider.center = new Vector3(0, 0.9f, 0);
        characterController.rigidbody.isKinematic = false;
    }

    public override void PhysicsUpdate()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
        {
            characterController.rigidbody.isKinematic = false;
            characterController.jumpState.PhysicsUpdate();
        }
    }

    public override void FrameUpdate()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
        {
            characterController.rigidbody.isKinematic = false;
            stateMachine.ChangeState(characterController.fallingState);
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            characterController.animationsManager.setAnimationToLedgeClimbing();
        }
    }

    public override void OnTrigger(TriggerType triggerType)
    {
        if (triggerType.Equals(TriggerType.CLIMBING_FINISHED))
        {
            stateMachine.ChangeState(characterController.runState);
        }
    }
}
