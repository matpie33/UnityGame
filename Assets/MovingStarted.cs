using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingStarted : StateMachineBehaviour
{
    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        CharacterController characterController = FindObjectOfType<CharacterController>();
        characterController.stateMachine.ChangeState(characterController.stateMachine.runState);
    }
}
