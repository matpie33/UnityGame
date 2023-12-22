using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class AnimationEventHandler : MonoBehaviour
{
    private CharacterController characterController;
    private GameObject key;

    [SerializeField]
    private Transform keyTargetPosition;

    [SerializeField]
    private GameObject rightHandObject;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void StepUpTeleport()
    {
        characterController.animationsManager.PlayMovingAnimation();
    }

    public void JumpStart()
    {
        characterController.stateMachine.ChangeState(characterController.stateMachine.jumpState);
    }

    public void DoorOpeningSpawnKey()
    {
        LockedDoor door = (LockedDoor)characterController.playerState.objectToInteractWith;
        characterController.playerBackpack.RemoveObject(door.requiredKey);
        key = Instantiate(door.requiredKey.model);
        key.transform.parent = keyTargetPosition.transform;
        key.transform.localPosition = Vector3.zero;
        key.transform.localRotation = Quaternion.Euler(9, -52, -81);
    }

    public void DoorOpeningDestroyKey()
    {
        Destroy(key);
        key = null;
    }

    public void DoorOpeningAnimationFinished()
    {
        characterController.playerState.objectToInteractWith.Interact(this);
    }

    public void PickingObjectsAttachToHand()
    {
        Pickable pickableObject = (Pickable)characterController.playerState.objectToInteractWith;
        characterController.playerBackpack.addObject(pickableObject);

        pickableObject.GetComponent<Collider>().enabled = false;
        pickableObject.transform.SetParent(rightHandObject.transform);
        pickableObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void PickingObjectsDestroyObject()
    {
        PlayerState playerState = characterController.playerState;
        Destroy(playerState.objectToInteractWith.gameObject);
        playerState.objectToInteractWith = null;
        playerState.isPickingObject = false;
    }

    public void ShimmyStart()
    {
        characterController.stateMachine.shimmyState.ShimmyMovingStart();
    }

    public void PullLeverStarts()
    {
        characterController.playerState.objectToInteractWith.Interact(null);
    }
}
