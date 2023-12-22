using System;
using UnityEditor;
using UnityEngine;

public class LedgeGrabState : State
{
    private const float expectedDistanceToWall = 0.18f;
    private CharacterController characterController;
    private StateMachine stateMachine;

    public LedgeGrabState(CharacterController characterController, StateMachine stateMachine)
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
        Vector3 directionToWall = wallData.directionFromPlayerToWall;
        characterController.transform.rotation = Quaternion.LookRotation(
            wallData.directionFromPlayerToWall,
            Vector3.up
        );
        float distanceToCollider = wallData.distanceToCollider;
        if (distanceToCollider < expectedDistanceToWall)
        {
            float valueToAdd = expectedDistanceToWall - distanceToCollider;
            characterController.transform.position =
                characterController.transform.position - directionToWall * valueToAdd;
        }
        else if (distanceToCollider > expectedDistanceToWall)
        {
            float valueToDecrease = distanceToCollider - expectedDistanceToWall;
            characterController.transform.position =
                characterController.transform.position + directionToWall * valueToDecrease;
        }
    }

    public override void ExitState()
    {
        characterController.animationsManager.disableRootMotion();
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
