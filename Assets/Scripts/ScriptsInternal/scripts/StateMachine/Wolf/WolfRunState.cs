using UnityEditor;
using UnityEngine;

public class WolfRunState : State
{
    private WolfStateMachine wolfStateMachine;

    public WolfRunState(WolfStateMachine stateMachine)
        : base()
    {
        this.wolfStateMachine = stateMachine;
    }

    public override void EnterState()
    {
        WolfAnimationsManager animationsManager = this.wolfStateMachine.wolfAnimationsManager;
        animationsManager.setAnimationToRun();
    }
}
