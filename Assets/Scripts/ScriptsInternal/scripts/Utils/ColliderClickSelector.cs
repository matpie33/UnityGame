using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ColliderClickSelector
{
    private static bool enabled = false;

    static ColliderClickSelector()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(10, 10, 200, 30));
        bool newEnabled = GUILayout.Toggle(enabled, " Collider Click Select", "Button");
        if (newEnabled != enabled)
        {
            enabled = newEnabled;
        }
        GUILayout.EndArea();

        Handles.EndGUI();

        if (!enabled)
            return;

        Event e = Event.current;

        if (e.type == UnityEngine.EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            if (Physics.Raycast(worldRay, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                Selection.activeGameObject = clickedObject;
                EditorGUIUtility.PingObject(clickedObject);

                e.Use();
            }
        }
    }
}