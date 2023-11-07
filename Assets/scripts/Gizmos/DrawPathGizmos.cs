using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DrawPathGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (Selection.Contains(gameObject))
        {
            DrawPathAndCubes();
            return;
        }

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (Selection.Contains(child.gameObject))
            {
                DrawPathAndCubes();
                break;
            }
        }
    }

    private void DrawPathAndCubes()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            Gizmos.color = Color.blue;
            float s = .3f;
            Vector3 thisChildPosition = child.position;
            Gizmos.DrawCube(thisChildPosition, new Vector3(s, s, s));

            if (i + 1 < gameObject.transform.childCount)
            {
                Transform nextChild = gameObject.transform.GetChild(i + 1);
                Vector3 nextChildPosition = nextChild.position;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(thisChildPosition, nextChildPosition);
            }
        }
    }
}
