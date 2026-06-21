
public class DeerBiteState : State
{
    private AnimalAnimationsManager animalAnimationsManager;

    public DeerBiteState(AnimalAnimationsManager animalAnimationsManager)
        : base()
    {
        this.animalAnimationsManager = animalAnimationsManager;
    }

    public override void EnterState()
    {
        animalAnimationsManager.setAnimationToBite();
    }
}
