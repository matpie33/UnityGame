using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class MovementState : State
{
    protected CharacterController characterController;
    private CameraController cameraController;
    protected PlayerStateMachine stateMachine;
    private Vector3 vectorNormalToGround = Vector3.zero;

    protected float targetSpeed;
    public Vector3 newVelocity { get; protected set; }
    private float newSpeed;

    private float moveSharpness = 10;
    protected Boolean playerMoving = true;

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

                characterController.currentWallHeight = objectsInFrontDetector.detectedObject
                    .GetComponent<Collider>()
                    .bounds.extents.y;
                characterController.GetComponent<Collider>().enabled = false;
                characterController.animationsManager.setAnimationToStepUp();
                characterController.rigidbody.isKinematic = true;
                stateMachine.ChangeState(new ClimbState(characterController, stateMachine));
                return;
            }
            else if (objectsInFrontDetector.detectedWallType.Equals(WallType.ABOVE_HIPS))
            {
                stateMachine.ChangeState(stateMachine.doingAnimationState);
                characterController.rigidbody.isKinematic = true;
                characterController.capsuleCollider.enabled = false;
                characterController.animationsManager.PlayMiddleWallClimb();
                return;
            }

            if (
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

        targetSpeed = _moveInputVector == Vector3.zero ? 0 : getTargetSpeed();
        if (targetSpeed == 0)
        {
            playerMoving = false;
        }
        else
        {
            playerMoving = true;
        }
        if (PlayerInputs.MoveAxisForwardRaw == -1)
        {
            targetSpeed = 1.3f;
        }

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
            newSpeed = 0;
        }
        else
        {
            newSpeed = Mathf.Lerp(newSpeed, targetSpeed, Time.deltaTime * moveSharpness);
        }

        newVelocity = _moveInputVector * newSpeed;
        newVelocity = Vector3.ProjectOnPlane(newVelocity, vectorNormalToGround);
        float slerpTime = 0.2f;
        if (targetSpeed != 0)
        {
            characterController.transform.forward = Vector3.Slerp(
                characterController.transform.forward,
                (
                    PlayerInputs.MoveAxisForwardRaw != 0
                        ? PlayerInputs.MoveAxisForwardRaw
                        : Mathf.Abs(PlayerInputs.MoveAxisRightRaw)
                ) * Vector3.ProjectOnPlane(_moveInputVector, vectorNormalToGround),
                slerpTime
            );
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
