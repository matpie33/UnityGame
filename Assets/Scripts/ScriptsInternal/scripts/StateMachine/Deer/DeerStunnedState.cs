
public class DeerStunnedState : State
{
    private AnimalAnimationsManager animalAnimationsManager;

    public DeerStunnedState(AnimalAnimationsManager animalAnimationsManager)
        : base()
    {
        this.animalAnimationsManager = animalAnimationsManager;
    }

    public override void EnterState()
    {
        animalAnimationsManager.setAnimationToStunned();
    }
}
