using System;
using UnityEngine;

public class AnimalAnimationsManager
{
    private Animator animator;

    private const string BASE_LAYER = "Base Layer";

    private enum Animation
    {
        run,
        idle,
        bite,
        stunned
    }

    private String anim(Animation animation)
    {
        return BASE_LAYER + "." + animation;
    }

    public AnimalAnimationsManager(Animator animator)
    {
        this.animator = animator;
    }

    public void setAnimationToRun()
    {
        animator.applyRootMotion = true;
        animator.CrossFade(anim(Animation.run), 0.1f);
    }

    public void setAnimationToStunned()
    {
        animator.CrossFade(anim(Animation.stunned), 0.1f);
    }

    public void setAnimationToIdle()
    {
        animator.CrossFade(anim(Animation.idle), 0.5f);
    }

    public void setAnimationToBite()
    {
        animator.applyRootMotion = true;
        animator.Play(anim(Animation.bite));
    }
}
