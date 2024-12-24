using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static bool DoesParentHaveComponent(GameObject gameObject, Type component)
    {
        if (gameObject != null && gameObject.transform.parent == null)
        {
            return false;
        }
        return gameObject.transform.parent.gameObject.GetComponent(component) != null;
    }
}
