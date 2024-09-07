using UnityEditor;
using UnityEngine;

public class WolfBiteState : State
{
    private WolfStateMachine wolfStateMachine;

    public WolfBiteState(WolfStateMachine stateMachine)
        : base()
    {
        this.wolfStateMachine = stateMachine;
    }

    public override void EnterState()
    {
        WolfAnimationsManager animationsManager = this.wolfStateMachine.wolfAnimationsManager;
        animationsManager.setAnimationToBite();
    }
}
