using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShimmyState : State
{
    private CharacterController characterController;
    private LedgeDirection ledgeDirection;
    private StateMachine stateMachine;
    private bool movingStarted = false;

    public ShimmyState(CharacterController characterController, StateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        movingStarted = false;
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

    public override void FrameUpdate()
    {
        LedgeContinuationDetector ledgeContinuationDetector =
            characterController.ledgeContinuationDetector;
        bool isSpaceForGrab = ledgeContinuationDetector.CheckIfThereIsSpaceForGrab(ledgeDirection);

        if (!isSpaceForGrab)
        {
            stateMachine.ChangeState(stateMachine.ledgeGrabState);
            return;
        }
        if (!movingStarted)
        {
            return;
        }

        bool ledgeContinues = ledgeContinuationDetector.CheckIfLedgeContinues(ledgeDirection);
        if (ledgeContinues)
        {
            characterController.transform.Translate(
                (int)ledgeDirection * characterController.transform.right * Time.deltaTime * 1.5f,
                Space.World
            );
        }
        else
        {
            if (ledgeContinuationDetector.CheckIfCanRotateAroundLedge(ledgeDirection))
            {
                RotateAroundLedge(ledgeDirection);
            }
            else
            {
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

    internal void ShimmyMovingStart()
    {
        movingStarted = true;
    }

    internal void ShimmyContinue(LedgeDirection ledgeDirection)
    {
        LedgeContinuationDetector ledgeContinuationDetector =
            characterController.ledgeContinuationDetector;

        bool ledgeContinues = ledgeContinuationDetector.CheckIfLedgeContinues(ledgeDirection);
        bool isSpaceForGrab = ledgeContinuationDetector.CheckIfThereIsSpaceForGrab(ledgeDirection);
        this.ledgeDirection = ledgeDirection;
        if (ledgeContinues && isSpaceForGrab)
        {
            EnterState();
        }
        else
        {
            if (ledgeContinuationDetector.CheckIfCanRotateAroundLedge(ledgeDirection))
            {
                RotateAroundLedge(ledgeDirection);
            }
            else
            {
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.ledgeGrabState
                );
            }
        }
    }
}
