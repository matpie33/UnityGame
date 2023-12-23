using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepUpDone : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 point = FindObjectOfType<ObjectsInFrontDetector>().verticalCollisionPosition;
        FindObjectOfType<CharacterController>().gameObject.transform.position = point;
    }
}
