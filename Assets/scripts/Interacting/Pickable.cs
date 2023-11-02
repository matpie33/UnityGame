using UnityEditor;
using UnityEngine;

public class Pickable : Interactable
{
    [field: SerializeField]
    public PickableDefinition definition { get; private set; }

    public override void Interact(Object data) { }
}
