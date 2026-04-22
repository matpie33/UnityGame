using System;
using UnityEngine;

public class Utils
{
    public static bool DoesParentHaveComponent(GameObject gameObject, Type component)
    {
        if (gameObject != null && gameObject.transform.parent == null)
        {
            return false;
        }
        return gameObject != null && gameObject.transform.parent.gameObject.GetComponent(component) != null;
    }
}
