using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAboveDetector : MonoBehaviour
{

    public bool isWallAbove { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        isWallAbove = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isWallAbove = false;
    }

   

}
