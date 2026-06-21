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
        animator.CrossFade(anim(Animation.run), 0.03f);
    }

    public void setAnimationToStunned()
    {
        animator.CrossFade(anim(Animation.stunned), 0.1f);
    }

    public void setAnimationToIdle()
    {
        animator.CrossFade(anim(Animation.idle), 0.1f);
    }

    public void setAnimationToBite()
    {
        animator.CrossFade(anim(Animation.bite), 0.1f);
    }
}
