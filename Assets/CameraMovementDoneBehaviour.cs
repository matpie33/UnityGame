using UnityEngine;

public class CameraMovementDoneBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        FindAnyObjectByType<CameraController>().EnableMe();
    }
}
