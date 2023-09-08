using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObjectsController : MonoBehaviour
{
    public bool hasObjectInFront { get; set; }

    private CharacterController characterController;

    public GameObject objectInFront { get; set; }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (hasObjectInFront && UnityEngine.Input.GetKeyDown(KeyCode.E))
        {
            characterController.PickupObject();
            hasObjectInFront = false;
        }
    }
}
