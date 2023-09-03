using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingDone : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("entered");
        animator.SetBool(AnimationVariables.IS_CLIMBING_LEDGE, false);
        animator.SetBool(AnimationVariables.IS_GROUNDED, true);
        animator.applyRootMotion = false;
        CharacterController charController = FindObjectOfType<CharacterController>();

        charController.setKinematicFalse();
    }
}
