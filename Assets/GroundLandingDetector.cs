using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLandingDetector : MonoBehaviour
{
    public bool isGrounded { get; private set; }

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
        characterController.doLand();
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
