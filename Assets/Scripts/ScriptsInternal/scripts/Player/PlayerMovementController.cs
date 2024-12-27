using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 targetDestination;

    private Vector3 destinationPart1;

    private bool targetDestinationInProgress;
    private bool destinationPart1InProgress;
    private bool rotatingInProgress;

    [SerializeField]
    private float stoppingDistance;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float rotationStoppingAngle;

    [SerializeField]
    private float movementSpeed;

    private Vector3 targetLookPosition;

    private bool movementTriggered;

    private EventQueue eventQueue;
    private Interactable objectToMoveTo;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        eventQueue = FindAnyObjectByType<EventQueue>();
    }

    public void SetMoveToDestination(
        Vector3 destination,
        Vector3 targetLookPosition,
        Interactable objectToMoveTo
    )
    {
        SetMoveToDestination(destination, targetLookPosition);
        this.objectToMoveTo = objectToMoveTo;
    }

    public void SetMoveToDestination(Vector3 destination, Vector3 targetLookPosition)
    {
        movementTriggered = true;
        this.targetLookPosition = targetLookPosition;
        rotatingInProgress = true;
        if (Mathf.Abs(destination.z - transform.position.z) > stoppingDistance)
        {
            destinationPart1 = new Vector3(
                destination.x,
                transform.position.y,
                transform.position.z
            );
            destinationPart1InProgress = true;
        }

        this.targetDestination = destination;
        characterController.stateMachine.ChangeState(
            characterController.stateMachine.doingAnimationState
        );
        targetDestinationInProgress = true;
    }

    private bool IsFarFromDistance(Vector3 destination)
    {
        return Vector3.Distance(destination, transform.position) > stoppingDistance; //sssrrrrqwe
    }

    void Update()
    {
        if (rotatingInProgress)
        {
            RotateTowardsDestination();
        }
        else if (destinationPart1InProgress)
        {
            if (IsFarFromDistance(destinationPart1))
            {
                MoveTowardsDestination(destinationPart1);
            }
            else
            {
                characterController.rigidbody.linearVelocity = Vector3.zero;
                destinationPart1InProgress = false;
                rotatingInProgress = true;
                characterController.animationsManager.SetAnimationToIdle();
            }
        }
        else if (targetDestinationInProgress && IsFarFromDistance(targetDestination))
        {
            MoveTowardsDestination(targetDestination);
        }
        else if (movementTriggered)
        {
            characterController.stateMachine.ChangeState(characterController.stateMachine.runState);
            characterController.animationsManager.setAnimationToMoving();
            targetDestinationInProgress = false;
            transform.LookAt(targetLookPosition);
            movementTriggered = false;
            eventQueue.SubmitEvent(
                new EventDTO(EventType.MOVEMENT_TOWARDS_TARGET_DONE, objectToMoveTo)
            );
        }
    }

    private void MoveTowardsDestination(Vector3 destination)
    {
        characterController.rigidbody.linearVelocity =
            (destination - transform.position).normalized * movementSpeed * Time.deltaTime;
    }

    private void RotateTowardsDestination()
    {
        Quaternion lookRotation = Quaternion.LookRotation(
            (destinationPart1InProgress ? destinationPart1 : targetDestination) - transform.position
        );
        if (Quaternion.Angle(lookRotation, transform.rotation) > rotationStoppingAngle)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * rotationSpeed
            );
        }
        else
        {
            rotatingInProgress = false;
            characterController.animationsManager.SetAnimationToWalk();
        }
    }
}
