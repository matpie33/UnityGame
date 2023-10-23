using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pickable/Object")]
public class PickableDefinition : ScriptableObject
{
    [field: SerializeField]
    public string objectName { get; private set; }

    [field: SerializeField]
    public string description { get; private set; }

    [field: SerializeField]
    public GameObject model { get; private set; }

    public override string ToString()
    {
        return objectName + ": " + description;
    }
}
