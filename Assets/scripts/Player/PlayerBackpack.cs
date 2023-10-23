using System.Collections;
using System.Collections.Generic;
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
}
