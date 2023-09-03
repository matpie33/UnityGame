using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    private LedgeController ledgeController;

    void Start()
    {
        ledgeController = FindObjectOfType<LedgeController>();
    }

    void Update() { }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("wall detected");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("wall gone");
        ledgeController.ledgeReadyToGrab();
    }
}
