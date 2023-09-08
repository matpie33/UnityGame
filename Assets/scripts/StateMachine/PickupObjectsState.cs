using UnityEditor;
using UnityEngine;

public class PickupObjectsState : State
{
    private float crouchSpeed = 3f;

    private CharacterController characterController;

    public PickupObjectsState(CharacterController characterController)
    {
        this.characterController = characterController;
    }

    public override void EnterState()
    {
        characterController.animationsManager.setAnimationToPickup();
    }
}
