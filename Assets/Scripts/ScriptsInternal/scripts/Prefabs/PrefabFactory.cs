using System.Collections;
using UnityEditor;
using UnityEngine;

public class PrefabFactory : MonoBehaviour
{
    [SerializeField]
    private GameObject questMarker;

    public GameObject GetPrefab(TypeOfPrefab prefabType, Vector3 position, Quaternion rotation)
    {
        GameObject prefab;
        switch (prefabType)
        {
            case TypeOfPrefab.QUEST_MARKER:
                prefab = Instantiate(questMarker, position, rotation);
                break;
            default:
                throw new System.Exception("Unknown prefab" + prefabType);
        }
        prefab.SetActive(true);
        return prefab;
    }
}
