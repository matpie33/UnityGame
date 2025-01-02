using UnityEngine;

public class WolfStunnedState : State
{
    private WolfStateMachine wolfStateMachine;

    public WolfStunnedState(WolfStateMachine stateMachine)
        : base()
    {
        this.wolfStateMachine = stateMachine;
    }

    public override void EnterState()
    {
        WolfAnimationsManager animationsManager = this.wolfStateMachine.wolfAnimationsManager;
        animationsManager.setAnimationToStunned();
    }
}
