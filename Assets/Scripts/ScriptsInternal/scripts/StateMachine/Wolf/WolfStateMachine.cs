
public class WolfStateMachine : AnimalStateMachine
{

    private void Start()
    {
        RunState = new WolfRunState(animalAnimationsManager);
        IdleState = new WolfIdleState(animalAnimationsManager);
        BiteState = new WolfBiteState(animalAnimationsManager);
        StunState = new WolfStunnedState(animalAnimationsManager);

        currentState = IdleState;
        currentState.EnterState();
    }
}
