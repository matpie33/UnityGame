using System;
using UnityEditor;
using UnityEngine;

public class LedgeGrabState : State
{
    private CharacterController characterController;
    private PlayerStateMachine stateMachine;

    public LedgeGrabState(CharacterController characterController, PlayerStateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        characterController.currentVelocity = Vector3.zero;
        characterController.rigidbody.isKinematic = true;
    }

    public override void ExitState()
    {
        characterController.animationsManager.disableRootMotion();
        characterController.rigidbody.isKinematic = false;
    }

    public override void FrameUpdate()
    {
        if (ActionKeys.IsKeyPressed(ActionKeys.LEDGE_RELEASE))
        {
            characterController.rigidbody.isKinematic = false;
            stateMachine.ChangeState(stateMachine.fallingState);
            stateMachine.fallingState.hasReleasedLedge = true;
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.CLIMB_LEDGE))
        {
            if (!characterController.canClimbUpWallChecker.isColliding)
            {
                characterController.animationsManager.setAnimationToLedgeClimbing();
            }
        }
        else if (PlayerInputs.left.PressedDown())
        {
            characterController.TryShimmy(LedgeDirection.LEFT);
        }
        else if (PlayerInputs.right.PressedDown())
        {
            characterController.TryShimmy(LedgeDirection.RIGHT);
        }
    }
}
