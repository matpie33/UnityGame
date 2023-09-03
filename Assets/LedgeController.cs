using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeController : MonoBehaviour
{
    private CharacterController compCharacterController;

    void Start()
    {
        compCharacterController = GetComponent<CharacterController>();
    }

    void Update() { }

    public void ledgeReadyToGrab()
    {
        compCharacterController.grabLedge();
    }
}
