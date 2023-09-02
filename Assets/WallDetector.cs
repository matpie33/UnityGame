using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    private LedgeController ledgeController;


    // Start is called before the first frame update
    void Start()
    {
        ledgeController = FindObjectOfType<LedgeController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
