
public class DeerRunState : State
{
    private AnimalAnimationsManager animalAnimationsManager;

    public DeerRunState(AnimalAnimationsManager animalAnimationsManager)
        : base()
    {
        this.animalAnimationsManager = animalAnimationsManager;
    }

    public override void EnterState()
    {
        animalAnimationsManager.setAnimationToRun();
    }
}
