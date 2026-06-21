
public class DeerStateMachine : AnimalStateMachine
{

    private void Start()
    {
        RunState=  new DeerRunState(animalAnimationsManager);
        IdleState = new DeerIdleState(animalAnimationsManager);
        BiteState = new DeerBiteState(animalAnimationsManager);
        StunState = new DeerStunnedState(animalAnimationsManager);

        currentState = IdleState;
        currentState.EnterState();
    }
}
