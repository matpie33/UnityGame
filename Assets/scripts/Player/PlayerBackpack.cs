using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerBackpack
{
    private List<Pickable> pickedObjects = new List<Pickable>();

    public void addObject(Pickable pickable)
    {
        pickedObjects.Add(pickable);
    }

    public List<Pickable> GetObjects()
    {
        return pickedObjects;
    }

    public bool HasObject(PickableDefinition definition)
    {
        return pickedObjects.Where(obj => obj.definition.Equals(definition)).Any();
    }

    public void RemoveObject(PickableDefinition definition)
    {
        pickedObjects.RemoveAll(obj => obj.definition.Equals(definition));
    }
}
