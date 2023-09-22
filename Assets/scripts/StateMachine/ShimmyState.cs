using System;
using UnityEditor;
using UnityEngine;

public class ShimmyState : State
{
    private CharacterController characterController;
    private int movementDirection;

    public ShimmyState(CharacterController characterController, StateMachine stateMachine)
    {
        this.characterController = characterController;
    }

    public override void EnterState()
    {
        characterController.rigidbody.isKinematic = true;
    }

    public override void FrameUpdate()
    {
        characterController.transform.Translate(
            movementDirection * characterController.transform.right * Time.deltaTime,
            Space.World
        );
    }

    internal void Direction(int direction)
    {
        movementDirection = direction;
    }
}
