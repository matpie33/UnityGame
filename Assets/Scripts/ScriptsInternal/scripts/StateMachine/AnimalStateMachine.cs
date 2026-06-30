using UnityEngine;

public abstract class AnimalStateMachine : StateMachine
{
    public State RunState { get; set; }
    public State IdleState { get; set; }
    public State BiteState { get; set; }
    public State StunState { get; set; }

    public bool UseRootMotionForRun { get; protected set; }

    public AnimalAnimationsManager animalAnimationsManager { get; private set; }

    private void Awake() {
        animalAnimationsManager = new AnimalAnimationsManager(GetComponentInParent<Animator>());
    }

    private void Start()
    {
        currentState = IdleState;
        currentState.EnterState();
    }
}
