using UnityEditor;
using UnityEngine;

public class WolfIdleState : State
{
    private WolfStateMachine wolfStateMachine;

    public WolfIdleState(WolfStateMachine stateMachine)
        : base()
    {
        this.wolfStateMachine = stateMachine;
    }

    public override void EnterState()
    {
        WolfAnimationsManager animationsManager = this.wolfStateMachine.wolfAnimationsManager;
        animationsManager.setAnimationToIdle();
    }
}
