using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectOnStart : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}
