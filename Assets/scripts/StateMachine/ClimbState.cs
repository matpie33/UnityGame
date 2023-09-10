using UnityEditor;
using UnityEngine;

public class ClimbState : State
{
    protected CharacterController characterController;
    protected StateMachine stateMachine;

    public ClimbState(CharacterController characterController, StateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }
}
