using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShimmyDoneBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController charController = FindObjectOfType<CharacterController>();
        if (charController.playerInputs.left.Pressed())
        {
            charController.TryShimmy(LedgeDirection.LEFT);
        }
        else if (charController.playerInputs.right.Pressed())
        {
            charController.TryShimmy(LedgeDirection.RIGHT);
        }
    }
}
