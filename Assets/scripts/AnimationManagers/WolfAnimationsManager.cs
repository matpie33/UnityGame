using System;
using System.Collections;
using UnityEngine;

public class WolfAnimationsManager
{
    private Animator animator;

    private const string BASE_LAYER = "Base Layer";

    private enum Animation
    {
        run,
        idle
    }

    private String anim(Animation animation)
    {
        return BASE_LAYER + "." + animation;
    }

    public WolfAnimationsManager(Animator animator)
    {
        this.animator = animator;
    }

    public void setAnimationToRun()
    {
        animator.CrossFade(anim(Animation.run), 0.1f);
    }

    public void setAnimationToIdle()
    {
        animator.CrossFade(anim(Animation.idle), 0.1f);
    }
}
