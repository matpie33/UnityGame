using System.Collections.Generic;
using System.Linq;
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

    [MenuItem("GameObject/Check for duplicated components instances")]
    static void CheckComponents()
    {
        List<GameObject> collidersGO = FindObjectsByType<BoxCollider>()
            .Select(collider => collider.gameObject)
            .ToList();

        collidersGO.ForEach(go =>
        {
            int boxColliders = go.GetComponents<BoxCollider>().Count();
            if (boxColliders > 1)
            {
                for (int i = 0; i < boxColliders - 1; i++)
                {
                    DestroyImmediate(go.GetComponent<BoxCollider>());
                }
            }
            int meshColliders = go.GetComponents<MeshCollider>().Count();
            if (meshColliders > 1)
            {
                for (int i = 0; i < meshColliders - 1; i++)
                {
                    DestroyImmediate(go.GetComponent<MeshCollider>());
                }
            }
            int colliders = go.GetComponents<Collider>().Count();
            if (colliders > 1)
            {
                Debug.LogError("Object has many colliders of different type: " + go.name);
            }
        });
    }

    [MenuItem("GameObject/Update box colliders %F3")]
    static void UpdateBoxColliders()
    {
        List<GameObject> collidersGO = FindObjectsByType<BoxCollider>()
            .Select(collider => collider.gameObject)
            .ToList();

        collidersGO.ForEach(go =>
        {
            if (!go.GetComponent<BoxCollider>().isTrigger)
            {
                PhysicsMaterial m = go.GetComponent<BoxCollider>().material;
                go.GetComponents<BoxCollider>()
                    .ToList()
                    .ForEach(collider => DestroyImmediate(collider));
                BoxCollider boxCollider = go.AddComponent<BoxCollider>();
                boxCollider.material = m;
            }
        });
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
                if (targetGameObject.GetComponent(component.GetType()) == null)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(component);
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);
                }
            }
        }
        copiedComponents = null;
    }

    static void CreateUuids(PlayModeStateChange playModeStateChange)
    {
        if (playModeStateChange.Equals(PlayModeStateChange.ExitingEditMode))
        {
            foreach (BaseObject o in FindObjectsByType<BaseObject>())
            {
                o.SetUuid(System.Guid.NewGuid().ToString());
            }
        }
    }
}
