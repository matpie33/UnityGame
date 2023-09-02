using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class CharacterController : MonoBehaviour
    {

        [Header("Movement")]
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

        private float targetSpeed;
        private Vector3 newVelocity;
        private Quaternion targetRotation;
        private float newSpeed;
        private Quaternion newRotation;

        public float jumpForce = 5;
        public float gravity = 4;

        public bool isGrounded = true;
        private bool isJumping;
        private bool isGrabbing;

        private void Start()
        {
            
            cameraController = GetComponent<CameraController>();
            playerInputs = GetComponent<PlayerInputs>();

            
            rigidBody = GetComponent<Rigidbody>();
            animationsManager = GetComponent<AnimationsManager>();

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
                Vector3 _moveInputVector = new Vector3(playerInputs.MoveAxisRightRaw, 0, playerInputs.MoveAxisForwardRaw).normalized;
                Vector3 _cameraPlanarDirection = cameraController.cameraPlanarDirection;
                Quaternion _cameraPlanarRotation = Quaternion.LookRotation(_cameraPlanarDirection);

                _moveInputVector = _cameraPlanarRotation * _moveInputVector;

                if (playerInputs.sprint.Pressed())
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
                    newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSharpness);
                    transform.rotation = newRotation;
                }
            }

            transform.Translate(newVelocity * Time.deltaTime, Space.World);
            animationsManager.setRunningSpeedParameter(newSpeed);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                if (isGrounded)
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

            rigidBody.AddForce(Vector3.up * -1 * gravity, ForceMode.Force);

            if ((isJumping && rigidBody.velocity.y < 0) || rigidBody.velocity.y < -2)
            {
                animationsManager.setAnimationToFalling();
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) && isGrabbing)
            {
                animationsManager.setAnimationToFalling();
                rigidBody.isKinematic = false;
                isGrabbing = false;
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

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("ground"))
            {
                isGrounded = true;                
                isJumping = false;
                animationsManager.setAnimationToGrounded();
            }
        }

}

