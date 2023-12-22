using System;
using UnityEditor;
using UnityEngine;

public class State
{
    public virtual void ExitState() { }

    public virtual void EnterState() { }

    public virtual void FrameUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void OnTrigger(TriggerType triggerType) { }

    internal virtual void LateUpdate() { }
}
