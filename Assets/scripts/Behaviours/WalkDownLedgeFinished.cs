using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class WalkDownLedgeFinished : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FindAnyObjectByType<CharacterController>().stateMachine.OnTriggerType(
            TriggerType.ENTER_LEDGE_GRAB_STATE
        );
    }
}
