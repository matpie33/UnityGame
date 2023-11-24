using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : Observer
{
    public bool canBeInteracted { get; set; } = true;

    public abstract void Interact(Object data);

    public override void OnEvent(EventDTO eventDTO) { }
}
