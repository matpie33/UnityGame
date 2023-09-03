using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsManager : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    public void setAnimationToFalling()
    {
        animator.SetBool(AnimationVariables.IS_FALLING, true);
        animator.SetBool(AnimationVariables.IS_GROUNDED, false);
        animator.SetBool(AnimationVariables.IS_GRABBING_LEDGE, false);
    }

    public void setAnimationToLedgeClimbing()
    {
        animator.applyRootMotion = true;
        animator.SetBool(AnimationVariables.IS_CLIMBING_LEDGE, true);
        animator.SetBool(AnimationVariables.IS_GRABBING_LEDGE, false);
    }

    public void setAnimationToLedgeGrab()
    {
        animator.SetBool(AnimationVariables.IS_GRABBING_LEDGE, true);
        animator.SetBool(AnimationVariables.IS_FALLING, false);
        animator.SetBool(AnimationVariables.IS_JUMPING, false);
    }

    public void setAnimationToGrounded()
    {
        animator.SetBool(AnimationVariables.IS_GROUNDED, true);
        animator.SetBool(AnimationVariables.IS_JUMPING, false);
        animator.SetBool(AnimationVariables.IS_FALLING, false);
    }

    public void setAnimationToJumping()
    {
        animator.SetBool(AnimationVariables.IS_JUMPING, true);
        animator.SetBool(AnimationVariables.IS_GROUNDED, false);
    }

    public void setRunningSpeedParameter(float speed)
    {
        animator.SetFloat("Forward", speed);
    }

    internal void setAnimationToWalk()
    {
        animator.SetBool(AnimationVariables.IS_CROUCHING, false);
    }

    internal void setAnimationToCrouch()
    {
        animator.SetBool(AnimationVariables.IS_CROUCHING, true);
    }
}
