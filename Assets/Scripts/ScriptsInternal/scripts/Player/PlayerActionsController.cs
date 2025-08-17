using UnityEngine;

public class PlayerActionsController : Observer
{
    private CharacterController characterController;
    private EventQueue eventQueue;

    [SerializeField]
    private float offsetForward;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        if (eventDTO.eventType.Equals(EventType.MOVEMENT_TOWARDS_TARGET_DONE))
        {
            if (eventDTO.eventData != null && eventDTO.eventData.GetType() == typeof(Lever))
            {
                PlayerAnimationsManager animationsManager = characterController.animationsManager;
                PlayerStateMachine stateMachine = characterController.stateMachine;
                animationsManager.setAnimationToPullLever();
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                Lever lever = (Lever)eventDTO.eventData;
                GameObject cameraPositionObject = lever.lookAtLeverCameraPosition;
                ObjectWithPositionDTO objectWithPositionDTO = new ObjectWithPositionDTO(
                    lever.gameObject,
                    cameraPositionObject.transform.position,
                    cameraPositionObject.transform.rotation
                );
                AnimationEventHandler animationEventHandler =
                    characterController.GetComponent<AnimationEventHandler>();
                animationEventHandler.SetRightHandTargetPosition(lever);
                eventQueue.SubmitEvent(
                    new EventDTO(EventType.LEVER_OPENING, objectWithPositionDTO)
                );
            }
        }
    }

    void Update()
    {
        PlayerState playerState = characterController.playerState;
        PlayerStateMachine stateMachine = characterController.stateMachine;
        PlayerAnimationsManager animationsManager = characterController.animationsManager;
        if (playerState.HasMedipacks() && ActionKeys.IsKeyPressed(ActionKeys.USE_MEDIPACK))
        {
            stateMachine.OnTriggerType(TriggerType.MEDIPACK_USED);
        }
        if (
            playerState.objectToInteractWith != null && ActionKeys.IsKeyPressed(ActionKeys.INTERACT)
        )
        {
            Interactable objectToInteractWith = playerState.objectToInteractWith;
            if (!objectToInteractWith.canBeInteracted)
            {
                return;
            }

            if (objectToInteractWith.GetType() == typeof(Lever))
            {
                Vector3 objectPositionFlat = new Vector3(
                    objectToInteractWith.transform.position.x,
                    transform.position.y,
                    objectToInteractWith.transform.position.z
                );
                Vector3 targetPosition =
                    objectPositionFlat - objectToInteractWith.transform.parent.right * offsetForward;


                GetComponent<PlayerMovementController>()
                    .SetMoveToDestination(targetPosition, objectPositionFlat, objectToInteractWith);
            }
            else if (objectToInteractWith.GetType() == typeof(WallPushButton))
            {
                animationsManager.SetAnimationToPushTheButton();
                stateMachine.ChangeState(stateMachine.doingAnimationState);
            }
            else if (objectToInteractWith.GetType() == typeof(Pickable))
            {
                animationsManager.setAnimationToPickup();
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                playerState.isPickingObject = true;
            }
            else if (objectToInteractWith.GetType() == typeof(LockedDoor))
            {
                LockedDoor door = (LockedDoor)playerState.objectToInteractWith;
                if (!door.PlayerHasKey())
                {
                    door.canBeInteracted = true;
                    return;
                }
                animationsManager.SetAnimationToOpenDoor();
                stateMachine.ChangeState(stateMachine.doingAnimationState);

                door.isOpened = true;
            }
            else if (objectToInteractWith.GetType() == typeof(GenericNpc))
            {
                objectToInteractWith.Interact(gameObject);
            }
            eventQueue.SubmitEvent(
                new EventDTO(EventType.INTERACTION_DONE, objectToInteractWith.gameObject)
            );
            objectToInteractWith.canBeInteracted = false;
        }
    }
}
