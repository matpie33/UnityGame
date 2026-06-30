
public class WolfStateMachine : AnimalStateMachine
{

    private void Start()
    {
        RunState = new WolfRunState(animalAnimationsManager);
        IdleState = new WolfIdleState(animalAnimationsManager);
        BiteState = new WolfBiteState(animalAnimationsManager);
        StunState = new WolfStunnedState(animalAnimationsManager);
        UseRootMotionForRun = false;

        currentState = IdleState;
        currentState.EnterState();
    }
}
