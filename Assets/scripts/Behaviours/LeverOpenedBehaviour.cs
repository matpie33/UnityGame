using UnityEngine;

public class LeverOpenedBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Lever>().SubmitEvent();
    }
}
