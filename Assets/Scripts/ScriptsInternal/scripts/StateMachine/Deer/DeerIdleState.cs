
public class DeerIdleState : State
{
    private AnimalAnimationsManager animalAnimationsManager;

    public DeerIdleState(AnimalAnimationsManager animalAnimationsManager)
        : base()
    {
        this.animalAnimationsManager = animalAnimationsManager;
    }

    public override void EnterState()
    {
        animalAnimationsManager.setAnimationToIdle();
    }
}
