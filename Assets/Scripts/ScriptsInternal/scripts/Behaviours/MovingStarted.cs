using UnityEngine;

public class MovingStarted : StateMachineBehaviour
{
    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        CharacterController characterController = FindAnyObjectByType<CharacterController>();
        characterController.stateMachine.ChangeState(characterController.stateMachine.runState);
    }
}
