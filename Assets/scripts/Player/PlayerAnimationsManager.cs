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
        jump_from_stand,
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
        ledge_rotate_right,
        walk_down_ledge,
        move_backward,
        open_door,
        running_jump,
        sliding,
        landing_from_run
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

    public void SetAnimationToOpenDoor()
    {
        animator.CrossFade(anim(AnimationName.open_door), 0.1f);
    }

    public void setAnimationToLedgeRotateLeft()
    {
        animator.updateMode = AnimatorUpdateMode.Fixed;
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.ledge_rotate_left));
    }

    public void setAnimationToLedgeRotateRight()
    {
        animator.updateMode = AnimatorUpdateMode.Fixed;
        animator.applyRootMotion = true;
        animator.Play(anim(AnimationName.ledge_rotate_right));
    }

    public void setAnimationToWalkDownLedge()
    {
        animator.applyRootMotion = true;
        animator.CrossFade(anim(AnimationName.walk_down_ledge), 0.1f);
    }

    public void setAnimationToLeftShimmy()
    {
        animator.Play(anim(AnimationName.left_shimmy));
    }

    public void setAnimationToRightShimmy()
    {
        animator.CrossFade(anim(AnimationName.right_shimmy), 0.1f);
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
        animator.applyRootMotion = false;
        animator.Play(AnimationName.step_up.ToString());
    }

    public void setAnimationToClimbMiddleLedge()
    {
        animator.applyRootMotion = true;
        animator.Play(AnimationName.climb_middle_ledge.ToString());
    }

    public void setAnimationToLandingFromRun()
    {
        animator.CrossFade(anim(AnimationName.landing_from_run), 0.1f);
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
        animator.CrossFade(anim(AnimationName.climbing), 0.1f);
    }

    public void setAnimationToHangingIdle()
    {
        animator.updateMode = AnimatorUpdateMode.Normal;
        animator.applyRootMotion = false;
        animator.CrossFade(anim(AnimationName.hanging_idle), 0.1f);
    }

    public void setAnimationToStandingJump()
    {
        animator.CrossFade(anim(AnimationName.jump_from_stand), 0.1f);
    }

    public void setRunningSpeedParameter(float speed)
    {
        animator.SetFloat("Forward", speed);
    }

    internal void setAnimationToMoving()
    {
        animator.CrossFade(anim(AnimationName.moving), 1f);
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
}
