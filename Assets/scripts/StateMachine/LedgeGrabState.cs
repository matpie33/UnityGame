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
        characterController.animationsManager.setAnimationToHangingIdle();
        characterController.GetWallData();

        WallData wallData = characterController.wallData;
        characterController.transform.rotation = Quaternion.LookRotation(
            wallData.directionFromPlayerToWall,
            Vector3.up
        );
        Vector3 point = new Vector3(
            wallData.horizontalCollisionPoint.x,
            wallData.verticalCollisionPoint.y,
            wallData.horizontalCollisionPoint.z
        );
        characterController.transform.position =
            point
            - Vector3.up
                * (
                    2 * characterController.capsuleCollider.bounds.extents.y
                    + characterController.upOffset
                )
            + characterController.transform.forward * characterController.forwardOffset;
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
