using UnityEngine;

public class StepUpDone : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharacterController characterController = FindAnyObjectByType<CharacterController>();
        characterController.GetComponent<Collider>().enabled = true;
    }
}
