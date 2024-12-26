using System;
using UnityEngine;

public abstract class Observer : BaseObject
{
    public abstract void OnEvent(EventDTO eventDTO);
}
