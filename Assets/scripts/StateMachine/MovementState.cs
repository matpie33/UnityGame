using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class MovementState : State
{
    protected CharacterController characterController;
    private CameraController cameraController;
    protected StateMachine stateMachine;

    protected float targetSpeed;
    public Vector3 newVelocity { get; protected set; }
    private Quaternion targetRotation;
    private float newSpeed;
    private Quaternion newRotation;

    private float rotationSharpness = 10;
    private float moveSharpness = 10;

    public MovementState(CharacterController characterController, StateMachine stateMachine)
    {
        this.characterController = characterController;
        this.stateMachine = stateMachine;
        cameraController = characterController.cameraController;
    }

    private bool IsDetectedObjectAWall()
    {
        return characterController.objectsInFrontDetector.detectedObject.GetComponent<NavMeshAgent>()
            == null;
    }

    public override void FrameUpdate()
    {
        if (this.GetType() != typeof(CrouchState) && ActionKeys.IsKeyPressed(ActionKeys.JUMP))
        {
            WallType detectedWallType = characterController.objectsInFrontDetector.detectedWallType;
            if (detectedWallType.Equals(WallType.BELOW_HIPS) && IsDetectedObjectAWall())
            {
                characterController.animationsManager.setAnimationToStepUp();
                characterController.rigidbody.isKinematic = true;
                stateMachine.ChangeState(new ClimbState(characterController, stateMachine));
                return;
            }
            if (detectedWallType.Equals(WallType.ABOVE_HIPS) && IsDetectedObjectAWall())
            {
                Vector3 position = characterController.transform.position;
                characterController.rigidbody.isKinematic = true;
                characterController.transform.position =
                    position + characterController.transform.forward * 0.007f;
                characterController.animationsManager.setAnimationToClimbMiddleLedge();
                stateMachine.ChangeState(new ClimbState(characterController, stateMachine));
                return;
            }

            JumpState jumpState = stateMachine.jumpState;
            stateMachine.ChangeState(jumpState);
        }

        Vector3 _moveInputVector = new Vector3(
            PlayerInputs.MoveAxisRightRaw,
            0,
            PlayerInputs.MoveAxisForwardRaw
        ).normalized;
        Vector3 _cameraPlanarDirection = cameraController.cameraPlanarDirection;
        Quaternion _cameraPlanarRotation = Quaternion.LookRotation(_cameraPlanarDirection);

        _moveInputVector = _cameraPlanarRotation * _moveInputVector;

        targetSpeed = _moveInputVector == Vector3.zero ? 0 : getTargetSpeed();
        if (PlayerInputs.MoveAxisForwardRaw == -1)
        {
            targetSpeed = 1f;
        }

        newSpeed = Mathf.Lerp(newSpeed, targetSpeed, Time.deltaTime * moveSharpness);
        newVelocity = _moveInputVector * newSpeed;
        if (targetSpeed != 0)
        {
            targetRotation = Quaternion.LookRotation(_moveInputVector);
            newRotation = Quaternion.Slerp(
                characterController.transform.rotation,
                PlayerInputs.MoveAxisForwardRaw != -1
                    ? targetRotation
                    : targetRotation * Quaternion.Euler(0, 180f, 0),
                Time.deltaTime * rotationSharpness
            );
            characterController.transform.rotation = newRotation;
        }
        if (PlayerInputs.MoveAxisForwardRaw != -1)
        {
            characterController.animationsManager.setRunningSpeedParameter(newSpeed);
        }
        else
        {
            characterController.animationsManager.setRunningSpeedParameter(-1);
        }
        Move(newVelocity);
        characterController.currentVelocity = newVelocity;
    }

    protected void Move(Vector3 newVelocity)
    {
        characterController.transform.Translate(newVelocity * Time.deltaTime, Space.World);
    }

    public abstract float getTargetSpeed();
}
