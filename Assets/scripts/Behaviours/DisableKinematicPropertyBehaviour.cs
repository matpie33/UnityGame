using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class DisableKinematicPropertyBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        CharacterController characterController = FindAnyObjectByType<CharacterController>();
        characterController.rigidbody.isKinematic = false;
        characterController.capsuleCollider.enabled = true;
    }
}
