using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAnimationFinished : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController charController = FindObjectOfType<CharacterController>();
        charController.pickupAnimationFinished();
    }
}
