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

    void Update()
    {
        GameObject objectInFront = characterController.obstacleDetector.obstacle;
        if (
            characterController.obstacleDetector.obstacleInFrontDetected
            && objectInFront != null
            && objectInFront.CompareTag(Tags.PUSHABLE)
            && ActionKeys.IsKeyPressed(ActionKeys.PUSH_OBJECT)
        )
        {
            inPushState = !inPushState;

            Rigidbody rb = objectInFront.GetComponent<Rigidbody>();
            if (inPushState)
            {
                rb.isKinematic = false;
                characterController.animationsManager.SetAnimationToPush();
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.doingAnimationState
                );
                rb.AddForce(transform.forward * pushForce, ForceMode.Force);
                pushedObject = objectInFront;
                transform.localScale = Vector3.one;
                characterController.transform.parent = pushedObject.transform;
                characterController.animationsManager.DisableRootMotion();
                characterController.rigidbody.isKinematic = true;
            }
            else
            {
                StopPushing();
            }
        }
        if (inPushState)
        {
            Rigidbody rb = pushedObject.GetComponent<Rigidbody>();
            if (ActionKeys.IsKeyHold(ActionKeys.LEFT_KEY))
            {
                pushedObject.transform.Rotate(new Vector3(0, ROTATION_SPEED, 0));
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(transform.forward * pushForce, ForceMode.Force);
            }
            if (ActionKeys.IsKeyHold(ActionKeys.RIGHT_KEY))
            {
                pushedObject.transform.Rotate(new Vector3(0, -ROTATION_SPEED, 0));
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(transform.forward * pushForce, ForceMode.Force);
            }

            if (rb.linearVelocity.y < -0.1f)
            {
                StopPushing();
            }
        }
    }

    public void StopPushing()
    {
        GameObject objectInFront = characterController.obstacleDetector.obstacle;
        Rigidbody rb = objectInFront.GetComponent<Rigidbody>();
        characterController.transform.parent = null;
        characterController.stateMachine.ChangeState(characterController.stateMachine.runState);
        inPushState = false;
        rb.isKinematic = true;

        characterController.animationsManager.setAnimationToMoving();
        pushedObject = null;
    }
}
