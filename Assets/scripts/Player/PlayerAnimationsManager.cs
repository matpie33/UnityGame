using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationsManager
{
    private Animator animator;

    private const string BASE_LAYER = "Base Layer";

    private enum AnimationName
    {
        middle_wall_climb,
        falling_from_stand,
        falling_from_run,
        climbing,
        standing_jump,
        moving,
        crouching,
        punching,
        kicking,
        Pickup,
        climb_middle_ledge,
        step_up,
        ledge_grab_idle,
        pull_lever,
        left_shimmy,
        right_shimmy,
        ledge_rotate_left,
        ledge_rotate_right,
        walk_down_ledge,
        move_backward,
        open_door,
        running_jump,
        sliding,
        landing_from_run,
        landing_from_stand,
        ledge_prepare_hold,
        dodge_right,
        dodge_left,
        push
    }

    public PlayerAnimationsManager(Animator animator)
    {
        this.animator = animator;
        animator.applyRootMotion = false;
    }

    public void SetAnimationToSliding()
    {
        animator.CrossFade(anim(AnimationName.sliding), 0.1f);
    }

    public void SetAnimationToDodgeRight()
    {
        animator.applyRootMotion = true;
        animator.CrossFade(anim(AnimationName.dodge_right), 0.1f);
    }

    public void SetAnimationToDodgeLeft()
    {
        animator.applyRootMotion = true;
        animator.CrossFade(anim(AnimationName.dodge_left), 0.1f);
    }

    public void SetAnimationToPush()
    {
        animator.applyRootMotion = true;
        animator.CrossFade(anim(AnimationName.push), 0.1f);
    }

    public void SetAnimationToOpenDoor()
    {
        animator.CrossFade(anim(AnimationName.open_door), 0.1f);
    }

    public void setAnimationToLedgeRotateLeft()
    {
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.ledge_rotate_left));
    }

    public void setAnimationToLedgeRotateRight()
    {
        animator.Play(anim(AnimationName.ledge_rotate_right));
    }

    public void setAnimationToWalkDownLedge()
    {
        animator.CrossFade(anim(AnimationName.walk_down_ledge), 0.1f);
    }

    public void setAnimationToLeftShimmy()
    {
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.left_shimmy));
    }

    public void setAnimationToRightShimmy()
    {
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.right_shimmy));
    }

    public void setAnimationToLedgePrepareHold()
    {
        animator.applyRootMotion = true;
        animator.CrossFade(anim(AnimationName.ledge_prepare_hold), 0.1f);
    }

    public void setAttackSpeed(float speed)
    {
        animator.SetFloat("AttackSpeed", speed);
    }

    public void setAnimationToStepUp()
    {
        animator.applyRootMotion = true;
        animator.Play(AnimationName.step_up.ToString());
    }

    public void setAnimationToClimbMiddleLedge()
    {
        animator.Play(AnimationName.climb_middle_ledge.ToString());
    }

    public void setAnimationToLandingFromRun()
    {
        animator.CrossFade(anim(AnimationName.landing_from_run), 0.1f);
    }

    public void setAnimationToLandingFromStand()
    {
        animator.Play(anim(AnimationName.landing_from_stand));
    }

    public void setAnimationToFallingFromStanding()
    {
        animator.CrossFade(anim(AnimationName.falling_from_stand), 0.1f);
    }

    public void setAnimationToFallingFromRunning()
    {
        animator.CrossFade(anim(AnimationName.falling_from_run), 0.1f);
    }

    public void setAnimationToRunningJump()
    {
        animator.CrossFade(anim(AnimationName.running_jump), 0.1f);
    }

    public void setAnimationToLedgeClimbing()
    {
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.climbing));
    }

    public void setAnimationToLedgeGrabIdle()
    {
        animator.CrossFade(anim(AnimationName.ledge_grab_idle), 0.1f);
    }

    public void setAnimationToStandingJump()
    {
        animator.CrossFade(anim(AnimationName.standing_jump), 0.1f);
    }

    public void setRunningSpeedParameter(float speed)
    {
        animator.SetFloat("Forward", speed);
    }

    internal void setAnimationToMoving()
    {
        animator.CrossFade(anim(AnimationName.moving), .3f);
    }

    internal void setAnimationToCrouch()
    {
        animator.CrossFade(anim(AnimationName.crouching), 0.4f);
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

    internal void setMovingBackward(bool moveBackward)
    {
        if (moveBackward)
        {
            animator.CrossFade(anim(AnimationName.move_backward), 0.1f);
        }
        animator.SetBool("MovingBack", moveBackward);
    }

    internal void PlayMovingAnimation()
    {
        animator.Play(anim(AnimationName.moving));
    }

    internal void PlayMiddleWallClimb()
    {
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.middle_wall_climb));
    }

    internal void DisableRootMotion()
    {
        animator.applyRootMotion = false;
    }
}
