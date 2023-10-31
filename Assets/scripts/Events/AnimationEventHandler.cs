using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AnimationEventHandler : MonoBehaviour
{
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void DoorOpeningSpawnKey()
    {
        characterController.SpawnKeyWhenOpeningDoor();
    }

    public void DoorOpeningDestroyKey()
    {
        characterController.DestroyKeyWhenOpeningDoor();
    }

    public void DoorOpeningAnimationFinished()
    {
        characterController.InteractWithDoor();
    }

    public void PickingObjectsAttachToHand()
    {
        characterController.AttachObjectToHand();
    }

    public void PickingObjectsDestroyObject()
    {
        characterController.DestroyPickedObject();
    }

    public void ShimmyStart()
    {
        characterController.ShimmyMovingStart();
    }

    public void PullLeverStarts()
    {
        characterController.PullLeverStarted();
    }
}
