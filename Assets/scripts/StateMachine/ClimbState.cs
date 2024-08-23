using UnityEditor;
using UnityEngine;

public class ClimbState : State
{
    protected CharacterController characterController;
    protected PlayerStateMachine stateMachine;

    public ClimbState(CharacterController characterController, PlayerStateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
    }
}
