using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeRotateFinished : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController characterController = FindAnyObjectByType<CharacterController>();
        characterController.stateMachine.OnTriggerType(TriggerType.ENTER_LEDGE_GRAB_STATE);
    }
}
