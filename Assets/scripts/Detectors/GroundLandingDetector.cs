using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLandingDetector : MonoBehaviour
{
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            characterController.GroundDetected();
        }
    }
}
