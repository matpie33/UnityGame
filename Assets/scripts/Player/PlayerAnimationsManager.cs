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
        braced_hang,
        jumping,
        moving,
        crouching,
        punching,
        kicking
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
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

    public void setAnimationToLedgeGrab()
    {
        animator.CrossFade(anim(AnimationName.braced_hang), 0.1f);
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

    private String anim(AnimationName animationName)
    {
        return BASE_LAYER + "." + animationName;
    }
}
