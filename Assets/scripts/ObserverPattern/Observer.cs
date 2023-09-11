using System;
using UnityEngine;

public abstract class Observer : MonoBehaviour
{
    public abstract void OnEvent(EventDTO eventDTO);
}
