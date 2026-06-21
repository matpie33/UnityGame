public class WolfIdleState : State
{
    private AnimalAnimationsManager animationsManager;

    public WolfIdleState(AnimalAnimationsManager animationsManager)
        : base()
    {
        this.animationsManager = animationsManager;
    }

    public override void EnterState()
    {
        animationsManager.setAnimationToIdle();
    }
}
