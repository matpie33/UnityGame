using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsManager : MonoBehaviour
{
    private Animator animator;

    private const string BASE_LAYER = "Base Layer";

    private enum AnimationName
    {
        falling,
        climbing,
        jumping,
        moving,
        crouching,
        punching,
        kicking,
        Pickup,
        climb_middle_ledge,
        step_up,
        hanging_idle,
        pull_lever,
        left_shimmy,
        right_shimmy,
        ledge_rotate_left,
        ledge_rotate_right
    }

    public void setAnimationToLedgeRotateLeft()
    {
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.ledge_rotate_left));
    }

    public void setAnimationToLedgeRotateRight()
    {
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.ledge_rotate_right));
    }

    public void setAnimationToLeftShimmy()
    {
        animator.CrossFade(anim(AnimationName.left_shimmy), 0.1f);
    }

    public void setAnimationToRightShimmy()
    {
        animator.CrossFade(anim(AnimationName.right_shimmy), 0.1f);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    public void setAttackSpeed(float speed)
    {
        animator.SetFloat("AttackSpeed", speed);
    }

    public void disableRootMotion()
    {
        animator.applyRootMotion = false;
    }

    public void setAnimationToStepUp()
    {
        animator.applyRootMotion = true;
        animator.Play(AnimationName.step_up.ToString());
    }

    public void setAnimationToClimbMiddleLedge()
    {
        animator.applyRootMotion = true;
        animator.Play(AnimationName.climb_middle_ledge.ToString());
    }

    public void setAnimationToFalling()
    {
        animator.CrossFade(anim(AnimationName.falling), 0.1f);
    }

    public void setAnimationToLedgeClimbing()
    {
        animator.applyRootMotion = true;
        animator.CrossFade(anim(AnimationName.climbing), 0.1f);
    }

    public void setAnimationToHangingIdle()
    {
        animator.applyRootMotion = false;
        animator.CrossFade(anim(AnimationName.hanging_idle), 0.1f);
    }

    public void setAnimationToJumping()
    {
        animator.CrossFade(anim(AnimationName.jumping), 0.1f);
    }

    public void setRunningSpeedParameter(float speed)
    {
        animator.SetFloat("Forward", speed);
    }

    internal void setAnimationToMoving()
    {
        animator.CrossFade(anim(AnimationName.moving), 0.1f);
    }

    internal void setAnimationToCrouch()
    {
        animator.CrossFade(anim(AnimationName.crouching), 0.06f);
    }

    public void setAnimationToPunch()
    {
        animator.CrossFade(anim(AnimationName.punching), 0.04f);
    }

    public void setAnimationToKick()
    {
        animator.CrossFade(anim(AnimationName.kicking), 0.1f);
    }

    public void setAnimationToPickup()
    {
        animator.CrossFade(anim(AnimationName.Pickup), 0.1f);
    }

    private String anim(AnimationName animationName)
    {
        return BASE_LAYER + "." + animationName;
    }

    internal void setAnimationToPullLever()
    {
        animator.CrossFade(anim(AnimationName.pull_lever), 0.1f);
    }
}
