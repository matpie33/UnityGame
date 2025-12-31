using UnityEngine;

public abstract class Interactable : Observer
{
    public bool canBeInteracted { get; set; } = true;

    public BaseObject GetBaseObject()
    {
        return this;
    }

    public abstract void Interact(Object data);

    public override void OnEvent(EventDTO eventDTO) { }

}
