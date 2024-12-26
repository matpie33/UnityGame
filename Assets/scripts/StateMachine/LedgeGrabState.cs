using System;
using UnityEditor;
using UnityEngine;

public class LedgeGrabState : State
{
    private CharacterController characterController;
    private PlayerStateMachine stateMachine;
    public Vector3 currentPlayerPosition;
    private GameObject ledge;

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
        if (ledge == null)
        {
            ledge = characterController.objectsInFrontDetector.detectedObject;
        }
        characterController.transform.LookAt(
            ledge.GetComponent<Collider>().ClosestPoint(characterController.transform.position)
        );
    }

    public override void ExitState()
    {
        characterController.rigidbody.isKinematic = false;
    }

    public override void FrameUpdate()
    {
        if (ledge == null)
        {
            stateMachine.ChangeState(stateMachine.fallingState);
            return;
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.LEDGE_RELEASE))
        {
            characterController.rigidbody.isKinematic = false;
            characterController.animationsManager.setAnimationToFallingFromStanding();
            stateMachine.ChangeState(stateMachine.fallingState);
            stateMachine.fallingState.releasedLedge = characterController
                .objectsInFrontDetector
                .detectedObject;
            ledge = null;
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.CLIMB_LEDGE))
        {
            if (!characterController.canClimbUpWallChecker.isColliding)
            {
                characterController.animationsManager.setAnimationToLedgeClimbing();
                characterController.capsuleCollider.enabled = false;
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.doingAnimationState
                );
                ledge = null;
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
