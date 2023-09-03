using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float crouchSpeed = 3f;

    [SerializeField]
    private float runSpeed = 6f;

    [SerializeField]
    private float sprintSpeed = 8f;

    [Header("Sharpness")]
    [SerializeField]
    private float rotationSharpness = 10;

    [SerializeField]
    private float moveSharpness = 10;

    private CameraController cameraController;
    private PlayerInputs playerInputs;
    private Rigidbody rigidBody;
    private AnimationsManager animationsManager;
    private CapsuleCollider capsuleCollider;
    private WallAboveDetector wallAboveDetector;

    private float targetSpeed;
    private Vector3 newVelocity;
    private Quaternion targetRotation;
    private float newSpeed;
    private Quaternion newRotation;

    public float jumpForce = 5;
    public float gravity = 4;
    private float initialHeight;

    public bool isGrounded = true;
    private bool isJumping;
    private bool isGrabbing;
    private bool isCrouching;

    private void Start()
    {
        cameraController = GetComponent<CameraController>();
        playerInputs = GetComponent<PlayerInputs>();
        wallAboveDetector = GetComponentInChildren<WallAboveDetector>();

        rigidBody = GetComponent<Rigidbody>();
        animationsManager = GetComponent<AnimationsManager>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        initialHeight = capsuleCollider.height;
    }

    public void setKinematicFalse()
    {
        rigidBody.isKinematic = false;
        isGrounded = true;
    }

    private void Update()
    {
        if (isGrounded)
        {
            Vector3 _moveInputVector = new Vector3(
                playerInputs.MoveAxisRightRaw,
                0,
                playerInputs.MoveAxisForwardRaw
            ).normalized;
            Vector3 _cameraPlanarDirection = cameraController.cameraPlanarDirection;
            Quaternion _cameraPlanarRotation = Quaternion.LookRotation(_cameraPlanarDirection);

            _moveInputVector = _cameraPlanarRotation * _moveInputVector;

            if (isCrouching)
            {
                targetSpeed = _moveInputVector != Vector3.zero ? crouchSpeed : 0;
            }
            else if (playerInputs.sprint.Pressed())
            {
                targetSpeed = _moveInputVector != Vector3.zero ? sprintSpeed : 0;
            }
            else
            {
                targetSpeed = _moveInputVector != Vector3.zero ? runSpeed : 0;
            }

            newSpeed = Mathf.Lerp(newSpeed, targetSpeed, Time.deltaTime * moveSharpness);
            newVelocity = _moveInputVector * newSpeed;
            if (targetSpeed != 0)
            {
                targetRotation = Quaternion.LookRotation(_moveInputVector);
                newRotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSharpness
                );
                transform.rotation = newRotation;
            }
        }

        transform.Translate(newVelocity * Time.deltaTime, Space.World);
        animationsManager.setRunningSpeedParameter(newSpeed);

        handleJumpAndLedgeClimb();

        rigidBody.AddForce(Vector3.up * -1 * gravity, ForceMode.Force);
        handleFallingDown();
        handleReleasingGrab();
        handleCrouching();
    }

    public void doLand()
    {
        isGrounded = true;
        isJumping = false;
        animationsManager.setAnimationToGrounded();
    }

    private void handleCrouching()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            if (isCrouching && !wallAboveDetector.isWallAbove)
            {
                isCrouching = false;
                animationsManager.setAnimationToWalk();
                changeHeight(true);
            }
            else
            {
                isCrouching = true;
                animationsManager.setAnimationToCrouch();
                changeHeight(false);
            }
        }
    }

    private void handleReleasingGrab()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) && isGrabbing)
        {
            animationsManager.setAnimationToFalling();
            rigidBody.isKinematic = false;
            isGrabbing = false;
        }
    }

    private void handleFallingDown()
    {
        if ((isJumping && rigidBody.velocity.y < 0) || rigidBody.velocity.y < -2)
        {
            animationsManager.setAnimationToFalling();
        }
    }

    private void handleJumpAndLedgeClimb()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && !isCrouching)
            {
                rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                isJumping = true;
                animationsManager.setAnimationToJumping();
            }
            else if (isGrabbing)
            {
                animationsManager.setAnimationToLedgeClimbing();
                isGrabbing = false;
            }
        }
    }

    public void changeHeight(bool toStanding)
    {
        if (toStanding)
        {
            capsuleCollider.height = initialHeight;
            capsuleCollider.center = new Vector3(0, 0.9f, 0);
        }
        else
        {
            capsuleCollider.height = initialHeight / 2;
            capsuleCollider.center = new Vector3(0, 0.47f, 0);
        }
    }

    public void grabLedge()
    {
        if (!isGrounded)
        {
            rigidBody.isKinematic = true;
            animationsManager.setAnimationToLedgeGrab();
            isGrabbing = true;
            isJumping = false;
        }
    }
}
