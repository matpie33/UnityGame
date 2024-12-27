using UnityEngine;

public class LedgeGrabStartingBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        CharacterController charController = FindAnyObjectByType<CharacterController>();
        charController.RotatePlayerTowardsWall();
    }
}
