
public class WolfBiteState : State
{
    private AnimalAnimationsManager animationsManager;

    public WolfBiteState(AnimalAnimationsManager animationsManager)
        : base()
    {
        this.animationsManager = animationsManager;
    }

    public override void EnterState()
    {
        animationsManager.setAnimationToBite();
    }
}
