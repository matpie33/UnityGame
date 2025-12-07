#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public static class FindUnusedObjects
{
    [MenuItem("Tools/Find Unused FBX Files in Scene")]
    private static void FindUnusedFBX()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (!scene.isLoaded)
        {
            Debug.LogError("No scene is currently loaded.");
            return;
        }

        string[] allFBXGuids = AssetDatabase.FindAssets("t:Model");
        HashSet<string> allFBXPaths = new HashSet<string>();
        foreach (string guid in allFBXGuids)
            allFBXPaths.Add(AssetDatabase.GUIDToAssetPath(guid));

        HashSet<string> usedAssets = new HashSet<string>();
        GameObject[] allObjects = scene.GetRootGameObjects();

        foreach (GameObject root in allObjects)
        {
            Object[] deps = EditorUtility.CollectDependencies(new Object[] { root });

            foreach (Object dep in deps)
            {
                string depPath = AssetDatabase.GetAssetPath(dep);
                if (!string.IsNullOrEmpty(depPath))
                    usedAssets.Add(depPath);
            }
        }

        List<string> unusedFBX = new List<string>();
        foreach (string fbx in allFBXPaths)
        {
            if (!usedAssets.Contains(fbx))
                unusedFBX.Add(fbx);
        }

        Debug.Log("===== Unused FBX Files in Scene =====");
        if (unusedFBX.Count == 0)
        {
            Debug.Log("All FBX files are used in this scene!");
            return;
        }

        foreach (string path in unusedFBX)
            Debug.Log(path);

        Debug.Log($"Total unused FBX files: {unusedFBX.Count}");
    }
}
#endif