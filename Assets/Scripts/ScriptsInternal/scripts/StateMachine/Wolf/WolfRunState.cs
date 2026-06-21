
public class WolfRunState : State
{
    private AnimalAnimationsManager animationsManager;

    public WolfRunState(AnimalAnimationsManager animationsManager)
        : base()
    {
        this.animationsManager = animationsManager;
    }

    public override void EnterState()
    {
        animationsManager.setAnimationToRun();
    }
}
