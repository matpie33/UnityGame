using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandToCrouchBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController characterController = FindObjectOfType<CharacterController>();
        characterController.changeHeight(false);
    }
}
