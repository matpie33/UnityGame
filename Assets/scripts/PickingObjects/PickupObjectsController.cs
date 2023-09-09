using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObjectsController : MonoBehaviour
{
    private CharacterController characterController;
    public GameObject objectInFront { get; set; }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (objectInFront != null && ActionKeys.IsKeyPressed(ActionKeys.PICKUP_OBJECT))
        {
            characterController.PickingObjectsStarted();
            objectInFront = null;
        }
    }
}
