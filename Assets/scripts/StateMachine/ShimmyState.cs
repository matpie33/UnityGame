using System;
using UnityEditor;
using UnityEngine;

public class ShimmyState : State
{
    private CharacterController characterController;
    private LedgeDirection ledgeDirection;
    private StateMachine stateMachine;
    private bool shimmyMovingStart;

    public ShimmyState(CharacterController characterController, StateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        characterController.rigidbody.isKinematic = true;
        shimmyMovingStart = false;
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
        if (!shimmyMovingStart)
        {
            return;
        }
        bool ledgeContinues = ledgeContinuationDetector.CheckIfLedgeContinues(ledgeDirection);
        if (ledgeContinues)
        {
            characterController.transform.Translate(
                (int)ledgeDirection * characterController.transform.right * Time.deltaTime * 2,
                Space.World
            );
        }
        else
        {
            bool canRotate = ledgeContinuationDetector.CheckIfCanRotateAroundLedge(ledgeDirection);
            if (canRotate)
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
        shimmyMovingStart = true;
    }

    internal void ShimmyContinue(LedgeDirection ledgeDirection)
    {
        this.ledgeDirection = ledgeDirection;
        EnterState();
    }
}
