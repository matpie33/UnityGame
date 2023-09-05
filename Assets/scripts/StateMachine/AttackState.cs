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
        if (UnityEngine.Input.GetKeyDown(KeyCode.P))
        {
            characterController.animationsManager.setAnimationToPunch();
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.K))
        {
            characterController.animationsManager.setAnimationToKick();
        }
    }
}
