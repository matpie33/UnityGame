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
            inPushState = true;
        }
    }

    void FixedUpdate()
    {
        GameObject objectInFront = characterController.objectsInFrontDetector.detectedObject;
        if (inPushState)
        {
            Rigidbody rb = pushedObject.GetComponent<Rigidbody>();
            if (rb.linearVelocity.y < -0.1f)
            {
                StopPushing();
                return;
            }

            rb.linearVelocity = transform.forward;

            characterController.rigidbody.linearVelocity = rb.linearVelocity;
            if (ActionKeys.IsKeyHold(ActionKeys.LEFT_KEY))
            {
                pushedObject.transform.Rotate(new Vector3(0, ROTATION_SPEED, 0));
                rb.linearVelocity = Vector3.zero;
            }
            if (ActionKeys.IsKeyHold(ActionKeys.RIGHT_KEY))
            {
                pushedObject.transform.Rotate(new Vector3(0, -ROTATION_SPEED, 0));
                rb.linearVelocity = Vector3.zero;
            }
        }
    }

    public void StopPushing()
    {
        characterController.rigidbody.linearVelocity = Vector3.zero;
        GameObject objectInFront = characterController.obstacleDetector.obstacle;
        Rigidbody rb = objectInFront.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        characterController.transform.parent = null;
        characterController.stateMachine.ChangeState(characterController.stateMachine.runState);

        characterController.animationsManager.setAnimationToMoving();
        pushedObject = null;
        inPushState = false;
    }
}
