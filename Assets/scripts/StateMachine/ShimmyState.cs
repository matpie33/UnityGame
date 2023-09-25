using System;
using UnityEditor;
using UnityEngine;

public class ShimmyState : State
{
    private CharacterController characterController;
    private LedgeDirection ledgeDirection;
    private StateMachine stateMachine;
    private bool isRotating;

    public ShimmyState(CharacterController characterController, StateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        characterController.rigidbody.isKinematic = true;
        isRotating = false;
    }

    public override void FrameUpdate()
    {
        if (isRotating)
        {
            return;
        }
        LedgeContinuationDetector ledgeContinuationDetector =
            characterController.ledgeContinuationDetector;
        bool ledgeContinues = ledgeContinuationDetector.CheckIfLedgeContinues(ledgeDirection);
        if (!ledgeContinues)
        {
            bool isLeftDirection = ledgeDirection.Equals(LedgeDirection.LEFT);
            bool canRotate = ledgeContinuationDetector.CheckIfCanRotateAroundLedge(ledgeDirection);
            if (canRotate)
            {
                if (isLeftDirection)
                {
                    characterController.animationsManager.setAnimationToLedgeRotateLeft();
                }
                else
                {
                    characterController.animationsManager.setAnimationToLedgeRotateRight();
                }
                characterController.GetWallData();
                isRotating = true;
                return;
            }
            stateMachine.ChangeState(stateMachine.ledgeGrabState);
            return;
        }
        characterController.transform.Translate(
            (int)ledgeDirection * characterController.transform.right * Time.deltaTime * 2,
            Space.World
        );
    }

    internal void Direction(LedgeDirection direction)
    {
        ledgeDirection = direction;
    }
}
