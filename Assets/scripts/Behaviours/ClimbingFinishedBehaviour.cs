using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingFinishedBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController charController = FindAnyObjectByType<CharacterController>();

        charController.ClimbingFinished();
    }
}
