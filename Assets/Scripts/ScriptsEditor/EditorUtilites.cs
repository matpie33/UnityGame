using System.Collections;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using Component = UnityEngine.Component;

[InitializeOnLoad]
public class EditorUtilites : EditorWindow
{
    static Component[] copiedComponents;

    static EditorUtilites()
    {
        EditorApplication.playModeStateChanged -= CreateUuids;
        EditorApplication.playModeStateChanged += CreateUuids;
    }

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

    static void CreateUuids(PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange.Equals(PlayModeStateChange.ExitingEditMode))
        {
            foreach (BaseObject o in FindObjectsByType<BaseObject>(FindObjectsSortMode.None))
            {
                o.SetUuid(System.Guid.NewGuid().ToString());
            }
        }
    }
}
