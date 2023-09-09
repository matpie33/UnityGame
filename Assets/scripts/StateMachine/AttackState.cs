using UnityEditor;
using UnityEngine;

public class AttackState : State
{
    private CharacterController characterController;

    public AttackState(CharacterController characterController)
    {
        this.characterController = characterController;
    }

    public override void EnterState()
    {
        if (ActionKeys.IsKeyPressed(ActionKeys.PUNCH))
        {
            characterController.animationsManager.setAnimationToPunch();
        }
        else if (ActionKeys.IsKeyPressed(ActionKeys.KICK))
        {
            characterController.animationsManager.setAnimationToKick();
        }
    }
}
