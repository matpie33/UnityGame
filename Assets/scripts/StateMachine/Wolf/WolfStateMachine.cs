using UnityEngine;

public class WolfStateMachine : StateMachine
{
    public WolfRunState wolfRunState { get; private set; }
    public WolfIdleState wolfIdleState { get; private set; }

    public WolfAnimationsManager wolfAnimationsManager { get; private set; }

    private void Awake() { }

    private void Start()
    {
        wolfRunState = new WolfRunState(this);
        wolfIdleState = new WolfIdleState(this);
        wolfAnimationsManager = new WolfAnimationsManager(GetComponentInParent<Animator>());

        currentState = wolfIdleState;
        currentState.EnterState();
    }
}
