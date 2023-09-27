using System;
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
        characterController.animationsManager.setAnimationToHangingIdle();
        WallData wallData = characterController.wallData;
        Vector3 directionToWall = wallData.directionFromPlayerToWall;
        characterController.transform.rotation = Quaternion.LookRotation(
            directionToWall,
            Vector3.up
        );
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
        if (ActionKeys.IsKeyPressed(ActionKeys.LEDGE_RELEASE))
        {
            characterController.rigidbody.isKinematic = false;
            stateMachine.jumpState.PhysicsUpdate();
        }
    }

    public override void FrameUpdate()
    {
        if (ActionKeys.IsKeyPressed(ActionKeys.LEDGE_RELEASE))
        {
            characterController.rigidbody.isKinematic = false;
            stateMachine.ChangeState(stateMachine.fallingState);
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.CLIMB_LEDGE))
        {
            characterController.animationsManager.setAnimationToLedgeClimbing();
        }
        else if (characterController.playerInputs.left.PressedDown())
        {
            characterController.TryShimmy(LedgeDirection.LEFT);
        }
        else if (characterController.playerInputs.right.PressedDown())
        {
            characterController.TryShimmy(LedgeDirection.RIGHT);
        }
    }
}
