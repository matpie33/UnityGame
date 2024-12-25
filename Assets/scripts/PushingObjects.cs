using UnityEngine;
using UnityEngine.UIElements;

public class PushingObjects : MonoBehaviour
{
    private CharacterController characterController;
    private bool inPushState;

    [SerializeField]
    private float pushForce;

    private Vector3 deltaPosition;

    private GameObject pushedObject;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        GameObject objectInFront = characterController.obstacleDetector.obstacle;
        if (
            characterController.obstacleDetector.obstacleInFrontDetected
            && objectInFront.CompareTag(Tags.PUSHABLE)
            && ActionKeys.IsKeyPressed(ActionKeys.PUSH_OBJECT)
        )
        {
            inPushState = !inPushState;

            if (inPushState)
            {
                Rigidbody rb = objectInFront.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * pushForce, ForceMode.Force);
                deltaPosition = transform.position - objectInFront.transform.position;
                characterController.animationsManager.SetAnimationToPush();
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.doingAnimationState
                );
                pushedObject = objectInFront;
            }
            else
            {
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.runState
                );
                characterController.animationsManager.setAnimationToMoving();
                pushedObject = null;
            }
        }
        if (inPushState)
        {
            Rigidbody rb = pushedObject.GetComponent<Rigidbody>();
            if (rb.angularDamping != 0)
            {
                rb.AddForce(
                    transform.forward * pushForce * rb.angularDamping / 50,
                    ForceMode.Force
                );
            }
            transform.position = pushedObject.transform.position + deltaPosition;
            if (rb.linearVelocity.y < -0.1f)
            {
                inPushState = false;
                characterController.stateMachine.ChangeState(
                    characterController.stateMachine.runState
                );
                characterController.animationsManager.setAnimationToMoving();
            }
        }
    }

    private void FixedUpdate()
    {
        RaycastHit result;

        Physics.Raycast(
            characterController.transform.position,
            characterController.transform.up * -1,
            out result,
            1f
        );

        if (result.collider != null)
        {
            Vector3 vectorNormalToGround = result.normal;
            transform.forward = Vector3.Slerp(
                transform.forward,
                Vector3.ProjectOnPlane(transform.forward, vectorNormalToGround),
                0.05f
            );
        }
    }
}
