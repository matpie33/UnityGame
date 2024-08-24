using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepUpDone : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 point = FindAnyObjectByType<ObjectsInFrontDetector>().verticalCollisionPosition;
        FindAnyObjectByType<CharacterController>().gameObject.transform.position = point;
    }
}
