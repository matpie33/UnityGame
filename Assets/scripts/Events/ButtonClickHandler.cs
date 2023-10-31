using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickHandler : MonoBehaviour
{
    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void ResetStats()
    {
        characterController.ResetStats();
    }

    public void AcceptStats()
    {
        characterController.AcceptStats();
    }
}
