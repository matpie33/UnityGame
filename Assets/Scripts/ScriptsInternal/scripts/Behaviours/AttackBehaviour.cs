using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        FindAnyObjectByType<CharacterController>().attackAnimationStart();
    }
}
