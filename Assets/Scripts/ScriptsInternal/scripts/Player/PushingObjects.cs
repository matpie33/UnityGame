using UnityEngine;

public class PushingObjects : MonoBehaviour
{
    private CharacterController characterController;
    private bool inPushState;

    [SerializeField]
    private float pushForce;

    private GameObject pushedObject;
    private const float ROTATION_SPEED = 1.3f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (inPushState && ActionKeys.IsKeyPressed(ActionKeys.PUSH_OBJECT))
        {
            inPushState = false;
            StopPushing();
            return;
        }
        GameObject objectInFront = characterController.objectsInFrontDetector.detectedObject;
        if (
            characterController.objectsInFrontDetector.obstacleFoundInFrontOfCamera
            && objectInFront != null
            && objectInFront.CompareTag(Tags.PUSHABLE)
            && ActionKeys.IsKeyPressed(ActionKeys.PUSH_OBJECT)
            && !inPushState
        )
        {
            characterController.stateMachine.ChangeState(
                characterController.stateMachine.doingAnimationState
            );

            pushedObject = objectInFront;
            transform.localScale = Vector3.one;

            characterController.animationsManager.SetAnimationToPush();
            characterController.transform.parent = pushedObject.transform;
            RigidbodyConstraints constraints = pushedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ;
            inPushState = true;
        }
    }

    void FixedUpdate()
    {
        if (inPushState)
        {
            characterController.rigidbody.isKinematic = true;
            Rigidbody pushedObjectRb = pushedObject.GetComponent<Rigidbody>();
            if (pushedObjectRb.linearVelocity.y < -0.1f)
            {
                StopPushing();
                return;
            }
            Vector3 targetPlayerVelocity = characterController.transform.forward;
            Vector3 currentPlayerVelocity = pushedObjectRb.linearVelocity;
            Vector3 velocityChange = targetPlayerVelocity - currentPlayerVelocity;

            float massDelta = pushedObjectRb.mass - characterController.rigidbody.mass;
            if (massDelta > 0)
            {
                velocityChange *= massDelta ;
            }

            pushedObjectRb.AddForceAtPosition(velocityChange, pushedObject.transform.position, ForceMode.Impulse);
        }
    }

    public void StopPushing()
    {
        Rigidbody rb = pushedObject.GetComponent<Rigidbody>();
        characterController.stateMachine.ChangeState(characterController.stateMachine.runState);
        characterController.transform.parent = null;
        characterController.animationsManager.setAnimationToMoving();
        RigidbodyConstraints constraints = rb.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
        pushedObject = null;
        inPushState = false;
    }
}
