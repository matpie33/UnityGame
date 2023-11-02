using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShimmyDoneBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController charController = FindObjectOfType<CharacterController>();
        if (PlayerInputs.left.Pressed())
        {
            charController.ShimmyContinue(LedgeDirection.LEFT);
        }
        else if (PlayerInputs.right.Pressed())
        {
            charController.ShimmyContinue(LedgeDirection.RIGHT);
        }
        else
        {
            charController.ShimmyDone();
        }
    }
}
