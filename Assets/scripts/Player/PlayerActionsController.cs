using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerActionsController : MonoBehaviour
{
    private CharacterController characterController;
    private EventQueue eventQueue;

    [SerializeField]
    private GameObject rightHandTarget;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        eventQueue = FindObjectOfType<EventQueue>();
    }

    void Update()
    {
        PlayerState playerState = characterController.playerState;
        StateMachine stateMachine = characterController.stateMachine;
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
                animationsManager.setAnimationToPullLever();
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                eventQueue.SubmitEvent(
                    new EventDTO(EventType.LEVER_OPENING, objectToInteractWith.gameObject)
                );
            }
            else if (objectToInteractWith.GetType() == typeof(Pickable))
            {
                animationsManager.setAnimationToPickup();
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                playerState.isPickingObject = true;
                rightHandTarget.transform.position = objectToInteractWith.transform.position;
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
                rightHandTarget.transform.position = door.lockTransform.position;

                door.isOpened = true;
            }
            else if (objectToInteractWith.GetType() == typeof(Npc))
            {
                objectToInteractWith.Interact(gameObject);
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.doingAnimationState
                );
            }
            eventQueue.SubmitEvent(new EventDTO(EventType.INTERACTION_DONE, null));
            objectToInteractWith.canBeInteracted = false;
        }
    }
}
