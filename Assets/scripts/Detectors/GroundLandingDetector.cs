using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLandingDetector : MonoBehaviour
{
    private CharacterController characterController;

    [field: SerializeField]
    private int collisionCount;

    private void Start()
    {
        characterController = GetComponentInParent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            collisionCount++;
            characterController.GroundDetected();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
        {
            collisionCount--;
        }
    }

    public bool IsHittingGround()
    {
        return collisionCount > 0;
    }
}
