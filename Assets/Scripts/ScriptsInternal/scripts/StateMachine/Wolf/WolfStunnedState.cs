
public class WolfStunnedState : State
{
    private AnimalAnimationsManager animationsManager;

    public WolfStunnedState(AnimalAnimationsManager animationsManager)
        : base()
    {
        this.animationsManager = animationsManager;
    }

    public override void EnterState()
    {
        animationsManager.setAnimationToStunned();
    }
}
