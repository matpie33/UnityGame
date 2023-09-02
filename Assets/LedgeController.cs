using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeController : MonoBehaviour
{
    private Animator animator;

    private CharacterController compCharacterController;

    void Start()
    {
        animator = GetComponent<Animator>();
        compCharacterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ledgeReadyToGrab()
    {
        
        compCharacterController.grabLedge();

    }

}
