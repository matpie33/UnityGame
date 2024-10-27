using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController charController = FindAnyObjectByType<CharacterController>();

        charController.ClimbingFinished();
    }

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.applyRootMotion = true;
        CharacterController characterController = FindAnyObjectByType<CharacterController>();
        characterController.SetPlayerPositionToWallHolding();
    }
}
