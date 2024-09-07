using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpenedBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Gate>().SubmitGateOpenedEvent();
    }
}
