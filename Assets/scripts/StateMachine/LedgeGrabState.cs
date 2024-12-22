using System;
using UnityEditor;
using UnityEngine;

public class LedgeGrabState : State
{
    private CharacterController characterController;
    private PlayerStateMachine stateMachine;
    public Vector3 currentPlayerPosition;

    public LedgeGrabState(CharacterController characterController, PlayerStateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        characterController.currentVelocity = Vector3.zero;
        characterController.rigidbody.isKinematic = true;
        characterController.ParentToRotatingObject();
        characterController.transform.position = currentPlayerPosition;
    }

    public override void ExitState()
    {
        characterController.rigidbody.isKinematic = false;
    }

    public override void FrameUpdate()
    {
        if (!characterController.IsPlayerParented)
        {
            characterController.SetPlayerPositionToWallHolding();
            characterController.transform.position = currentPlayerPosition;
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.LEDGE_RELEASE))
        {
            characterController.rigidbody.isKinematic = false;
            characterController.animationsManager.setAnimationToFallingFromStanding();
            stateMachine.ChangeState(stateMachine.fallingState);
            stateMachine.fallingState.releasedLedge = characterController
                .objectsInFrontDetector
                .detectedObject;
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.CLIMB_LEDGE))
        {
            if (!characterController.canClimbUpWallChecker.isColliding)
            {
                characterController.animationsManager.setAnimationToLedgeClimbing();
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.doingAnimationState
                );
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
