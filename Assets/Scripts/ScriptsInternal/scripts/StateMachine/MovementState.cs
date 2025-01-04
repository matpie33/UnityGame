using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class MovementState : State
{
    protected CharacterController characterController;
    private CameraController cameraController;
    protected PlayerStateMachine stateMachine;
    public Vector3 vectorNormalToGround { get; private set; }

    protected float targetSpeed;
    public Vector3 newVelocity { get; protected set; }
    private float newSpeed;

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
            HandleJump();
            return;
        }

        Vector3 _moveInputVector = new Vector3(
            PlayerInputs.MoveAxisRightRaw,
            0,
            PlayerInputs.MoveAxisForwardRaw
        ).normalized;
        Vector3 _cameraPlanarDirection = cameraController.cameraPlanarDirection;
        Quaternion _cameraPlanarRotation = Quaternion.LookRotation(_cameraPlanarDirection);

        _moveInputVector = _cameraPlanarRotation * _moveInputVector;
        if (_moveInputVector != Vector3.zero)
        {
            characterController.rotationTarget = PlayerRotationTarget.MOVEMENT_DIRECTION;
        }
        else if (characterController.objectToRotateTo != null)
        {
            characterController.rotationTarget = PlayerRotationTarget.ENEMY;
        }

        targetSpeed = _moveInputVector == Vector3.zero ? 0 : getTargetSpeed();
        if (PlayerInputs.MoveAxisForwardRaw == -1)
        {
            targetSpeed = 1.3f;
        }

        if (
            characterController.objectsInFrontDetector.obstacleFoundInFrontOfCamera
            && PlayerInputs.MoveAxisForwardRaw != -1
        )
        {
            newSpeed = Mathf.Lerp(newSpeed, 0, Time.deltaTime * characterController.moveSharpness);
        }
        else if (
            characterController.objectsInFrontDetector.obstacleBehindPlayerDetected
            && PlayerInputs.MoveAxisForwardRaw == -1
        )
        {
            newSpeed = 0;
        }
        else
        {
            newSpeed = Mathf.Lerp(
                newSpeed,
                targetSpeed,
                Time.deltaTime * characterController.moveSharpness
            );
        }

        newVelocity = _moveInputVector * newSpeed;

        switch (characterController.rotationTarget)
        {
            case PlayerRotationTarget.MOVEMENT_DIRECTION:
                if (targetSpeed != 0)
                {
                    characterController.transform.forward = Vector3.Slerp(
                        characterController.transform.forward,
                        (
                            PlayerInputs.MoveAxisForwardRaw != 0
                                ? PlayerInputs.MoveAxisForwardRaw
                                : Mathf.Abs(PlayerInputs.MoveAxisRightRaw)
                        ) * _moveInputVector,
                        characterController.rotationSharpness * Time.deltaTime
                    );
                }
                break;

            case PlayerRotationTarget.ENEMY:
                characterController.transform.rotation = Quaternion.Slerp(
                    characterController.transform.rotation,
                    Quaternion.LookRotation(
                        characterController.objectToRotateTo.transform.position
                            - characterController.transform.position
                    ),
                    characterController.rotationSharpness * Time.deltaTime
                );
                break;
        }

        if (PlayerInputs.MoveAxisForwardRaw != -1)
        {
            characterController.animationsManager.setRunningSpeedParameter(newSpeed);
        }
        else
        {
            characterController.animationsManager.setRunningSpeedParameter(-newSpeed);
        }
        characterController.currentVelocity = newVelocity;
    }

    private void HandleJump()
    {
        ObjectsInFrontDetector objectsInFrontDetector = characterController.objectsInFrontDetector;
        WallType detectedWallType = objectsInFrontDetector.detectedWallType;
        if (detectedWallType.Equals(WallType.BELOW_HIPS) && IsDetectedObjectAWall())
        {
            Vector3 verticalCollisionPoint = objectsInFrontDetector.verticalCollisionPosition;

            Vector3 playerPosition = characterController.transform.position;
            characterController.currentWallHeight =
                objectsInFrontDetector.verticalCollisionPosition.y
                - characterController.transform.position.y;

            characterController.GetComponent<Collider>().enabled = false;
            characterController.animationsManager.setAnimationToStepUp();
            stateMachine.ChangeState(new ClimbState(characterController, stateMachine));
            characterController.rigidbody.isKinematic = true;
        }
        else if (objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HIPS))
        {
            stateMachine.ChangeState(stateMachine.doingAnimationState);
            characterController.rigidbody.isKinematic = true;
            characterController.capsuleCollider.enabled = false;
            characterController.animationsManager.PlayMiddleWallClimb();
        }
        else if (
            newVelocity.magnitude < 0.01f || objectsInFrontDetector.obstacleFoundInFrontOfCamera
        )
        {
            characterController.animationsManager.setAnimationToStandingJump();
        }
        else
        {
            characterController.animationsManager.setAnimationToRunningJump();
            stateMachine.ChangeState(stateMachine.jumpState);
        }
    }

    public override void PhysicsUpdate()
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
            vectorNormalToGround = result.normal;
        }

        Move(newVelocity);
    }

    public override void ExitState()
    {
        base.ExitState();
        newVelocity = Vector3.zero;
        Move(newVelocity);
    }

    protected void Move(Vector3 newVelocity)
    {
        if (characterController.objectsInFrontDetector.isCollidingWithGround)
        {
            characterController.rigidbody.linearVelocity = new Vector3(
                newVelocity.x,
                characterController.rigidbody.linearVelocity.y,
                newVelocity.z
            );
        }
    }

    public abstract float getTargetSpeed();
}
