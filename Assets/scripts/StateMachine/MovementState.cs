using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public abstract class MovementState : State
{
    protected CharacterController characterController;
    private CameraController cameraController;
    protected PlayerStateMachine stateMachine;

    protected float targetSpeed;
    public Vector3 newVelocity { get; protected set; }
    private Quaternion targetRotation;
    private float newSpeed;
    private Quaternion newRotation;

    private float rotationSharpness = 10;
    private float moveSharpness = 10;

    public MovementState(CharacterController characterController, PlayerStateMachine stateMachine)
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
            ObjectsInFrontDetector objectsInFrontDetector =
                characterController.objectsInFrontDetector;
            WallType detectedWallType = objectsInFrontDetector.detectedWallType;
            if (detectedWallType.Equals(WallType.BELOW_HIPS) && IsDetectedObjectAWall())
            {
                Vector3 verticalCollisionPoint = objectsInFrontDetector.verticalCollisionPosition;
                characterController.SetLegAndHipTarget(verticalCollisionPoint);
                characterController.animationsManager.setAnimationToStepUp();
                characterController.rigidbody.isKinematic = true;
                stateMachine.ChangeState(new ClimbState(characterController, stateMachine));
                return;
            }

            if (newVelocity.magnitude == 0)
            {
                characterController.animationsManager.setAnimationToStandingJump();
            }
            else
            {
                characterController.animationsManager.setAnimationToRunningJump();
                stateMachine.ChangeState(stateMachine.jumpState);
            }
            return;
        }
        if (
            characterController.objectsInFrontDetector.obstacleFoundInFrontOfCamera
            && PlayerInputs.MoveAxisForwardRaw != -1
        )
        {
            newSpeed = Mathf.Lerp(newSpeed, 0, Time.deltaTime * moveSharpness);
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

        float speedBackward = -1;
        if (
            characterController.objectsInFrontDetector.obstacleFoundInFrontOfCamera
            && PlayerInputs.MoveAxisForwardRaw != -1
        )
        {
            newSpeed = Mathf.Lerp(newSpeed, 0, Time.deltaTime * moveSharpness);
        }
        else if (
            characterController.objectsInFrontDetector.obstacleBehindPlayerDetected
            && PlayerInputs.MoveAxisForwardRaw == -1
        )
        {
            speedBackward = 0;
            newSpeed = 0;
        }
        else
        {
            newSpeed = Mathf.Lerp(newSpeed, targetSpeed, Time.deltaTime * moveSharpness);
        }

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
            characterController.animationsManager.setRunningSpeedParameter(speedBackward);
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
