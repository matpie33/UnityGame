using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShimmyState : State
{
    private CharacterController characterController;
    private LedgeDirection ledgeDirection;
    private PlayerStateMachine stateMachine;
    private Vector3 currentPlayerPosition;

    public ShimmyState(CharacterController characterController, PlayerStateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        characterController.rigidbody.isKinematic = true;
        switch (ledgeDirection)
        {
            case LedgeDirection.LEFT:
                characterController.animationsManager.setAnimationToLeftShimmy();
                break;
            case LedgeDirection.RIGHT:
                characterController.animationsManager.setAnimationToRightShimmy();
                break;
        }
    }

    public override void ExitState()
    {
        stateMachine.ledgeGrabState.currentPlayerPosition = currentPlayerPosition;
    }

    public override void FrameUpdate()
    {
        LedgeContinuationDetector ledgeContinuationDetector =
            characterController.ledgeContinuationDetector;

        if (
            !(PlayerInputs.left.Pressed() && ledgeDirection.Equals(LedgeDirection.LEFT))
            && !(PlayerInputs.right.Pressed() && ledgeDirection.Equals(LedgeDirection.RIGHT))
        )
        {
            characterController.animationsManager.setAnimationToLedgeGrabIdle();
            stateMachine.ChangeState(stateMachine.ledgeGrabState);
            return;
        }

        bool ledgeContinues = ledgeContinuationDetector.CheckIfLedgeContinues(ledgeDirection);
        if (!ledgeContinues)
        {
            if (ledgeContinuationDetector.CheckIfCanRotateAroundLedge(ledgeDirection))
            {
                RotateAroundLedge(ledgeDirection);
            }
            else
            {
                characterController.animationsManager.setAnimationToLedgeGrabIdle();
                stateMachine.ChangeState(stateMachine.ledgeGrabState);
            }
        }
    }

    private void RotateAroundLedge(LedgeDirection direction)
    {
        LedgeContinuationDetector ledgeContinuationDetector =
            characterController.ledgeContinuationDetector;
        switch (direction)
        {
            case LedgeDirection.LEFT:
                characterController.animationsManager.setAnimationToLedgeRotateLeft();
                break;
            case LedgeDirection.RIGHT:
                characterController.animationsManager.setAnimationToLedgeRotateRight();
                break;
        }
        Vector3 newDirection = ledgeContinuationDetector.GetRotatedVector(
            characterController.wallData.directionFromPlayerToWall,
            direction
        );
        characterController.wallData.directionFromPlayerToWall = newDirection;
        stateMachine.ChangeState(stateMachine.doingAnimationState);
    }

    internal void Direction(LedgeDirection direction)
    {
        ledgeDirection = direction;
    }
}
