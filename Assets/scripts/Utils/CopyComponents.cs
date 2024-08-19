using System.Collections;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;

public class CopyComponents : EditorWindow
{
    static Component[] copiedComponents;

    [MenuItem("GameObject/Copy all components")]
    static void Copy()
    {
        copiedComponents = Selection.activeGameObject.GetComponents<Component>();
    }

    [MenuItem("GameObject/Paste all components")]
    static void Paste()
    {
        if (copiedComponents == null)
        {
            return;
        }
        Debug.Log("paste called");
        foreach (GameObject targetGameObject in Selection.gameObjects)
        {
            if (!targetGameObject)
                continue;
            foreach (Component component in copiedComponents)
            {
                if (!component)
                    continue;
                UnityEditorInternal.ComponentUtility.CopyComponent(component);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
            }
        }
        copiedComponents = null;
    }
}
