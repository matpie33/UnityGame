using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController charController = FindAnyObjectByType<CharacterController>();
        charController.animationsManager.DisableRootMotion();
        charController.ClimbingFinished();
    }
}
