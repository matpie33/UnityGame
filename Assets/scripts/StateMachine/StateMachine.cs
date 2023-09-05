using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public State currentState { get; private set; }

    public void Initialize(State initialState)
    {
        currentState = initialState;
        currentState.EnterState();
    }

    public void ChangeState(State state)
    {
        currentState.ExitState();
        currentState = state;
        state.EnterState();
    }
}
