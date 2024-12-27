using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class AnimationEventHandler : Observer
{
    private CharacterController characterController;
    private GameObject key;

    [SerializeField]
    private Transform keyTargetPosition;

    [SerializeField]
    private GameObject rightHandObject;

    [SerializeField]
    [FormerlySerializedAs("rigTarget")]
    private GameObject leftHandTarget;

    [SerializeField]
    private GameObject rightHandTarget;

    private GameObject rightHandTargetObject;

    [SerializeField]
    private TwoBoneIKConstraint rightHandRig;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (rightHandTargetObject != null)
        {
            rightHandTarget.transform.position = rightHandTargetObject.transform.position;
        }
    }

    public void ClearRightHandRigWeight()
    {
        rightHandRig.weight = 0;
    }

    public void SetRightHandTargetPosition(Lever lever)
    {
        rightHandTargetObject = lever.transform.parent.Find("Armature/Bone/Target").gameObject;
    }

    public void JumpStart()
    {
        characterController.stateMachine.ChangeState(characterController.stateMachine.jumpState);
    }

    public void DoorOpeningSpawnKey()
    {
        LockedDoor door = (LockedDoor)characterController.playerState.objectToInteractWith;
        characterController.playerBackpack.RemoveObject(
            door.requiredKey.GetComponent<PickableDefinition>()
        );
        key = Instantiate(door.requiredKey);
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

    public void PickinObjectsAttachToHand()
    {
        Pickable pickableObject = (Pickable)characterController.playerState.objectToInteractWith;

        pickableObject.GetComponent<Collider>().enabled = false;
        pickableObject.transform.SetParent(rightHandObject.transform);
        pickableObject.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void PickingObjectsSetRigTarget()
    {
        Pickable pickableObject = (Pickable)characterController.playerState.objectToInteractWith;
        characterController.playerBackpack.addObject(pickableObject);

        leftHandTarget.transform.position = pickableObject.gameObject.transform.position;
    }

    public void PickingObjectsDestroyObject()
    {
        PlayerState playerState = characterController.playerState;
        playerState.objectToInteractWith.gameObject.SetActive(false);
        playerState.objectToInteractWith = null;
        playerState.isPickingObject = false;
    }

    public void PullLeverStarts()
    {
        Lever lever = (Lever)characterController.playerState.objectToInteractWith;
        lever.Interact(gameObject);
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        if (eventDTO.eventType.Equals(EventType.GATE_OPENED))
        {
            ClearRightHandRigWeight();
        }
    }
}
