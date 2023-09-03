using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchToStandBehaviour : StateMachineBehaviour
{
   

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController characterController = FindObjectOfType<CharacterController>();
        characterController.changeHeight(true);
    }

    
}
