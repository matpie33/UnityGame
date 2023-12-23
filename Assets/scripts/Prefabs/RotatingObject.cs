using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, 1, 0));
    }
}
