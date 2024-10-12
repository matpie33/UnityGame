using NUnit.Framework.Interfaces;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    public State currentState { get; set; }

    public void ChangeState(State state)
    {
        if (currentState == state)
        {
            return;
        }
        currentState.ExitState();
        currentState = state;
        state.EnterState();
    }

    private void LateUpdate()
    {
        currentState.LateUpdate();
    }

    protected void BaseUpdate()
    {
        currentState.FrameUpdate();
    }
}
