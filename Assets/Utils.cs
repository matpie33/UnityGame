using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static bool DoesParentHaveTag(GameObject gameObject, string tag)
    {
        if (gameObject.transform.parent == null)
        {
            return false;
        }
        return gameObject.transform.parent.gameObject.CompareTag(tag);
    }
}
