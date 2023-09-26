using System;
using UnityEditor;
using UnityEngine;

public class ShimmyState : State
{
    private CharacterController characterController;
    private LedgeDirection ledgeDirection;
    private StateMachine stateMachine;

    public ShimmyState(CharacterController characterController, StateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }

    public override void EnterState()
    {
        characterController.rigidbody.isKinematic = true;
    }

    public override void FrameUpdate()
    {
        LedgeContinuationDetector ledgeContinuationDetector =
            characterController.ledgeContinuationDetector;
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
                characterController.RotateAroundLedge(ledgeDirection);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.ledgeGrabState);
            }
        }
    }

    internal void Direction(LedgeDirection direction)
    {
        ledgeDirection = direction;
    }

    internal void ShimmyAgain(LedgeDirection direction)
    {
        LedgeContinuationDetector ledgeContinuationDetector =
            characterController.ledgeContinuationDetector;
        bool ledgeContinues = ledgeContinuationDetector.CheckIfLedgeContinues(ledgeDirection);
        if (ledgeContinues)
        {
            switch (direction)
            {
                case LedgeDirection.LEFT:
                    characterController.animationsManager.setAnimationToLeftShimmy();
                    break;
                case LedgeDirection.RIGHT:
                    characterController.animationsManager.setAnimationToRightShimmy();
                    break;
            }
        }
    }
}
