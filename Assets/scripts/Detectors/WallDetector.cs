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

    private void OnTriggerExit(Collider other)
    {
        ledgeController.ledgeReadyToGrab();
    }
}
