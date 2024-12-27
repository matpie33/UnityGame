using UnityEditor;
using UnityEngine;

public class ObjectWithPositionDTO
{
    public GameObject gameObject { get; private set; }
    public Vector3 position { get; private set; }
    public Quaternion rotation { get; private set; }

    public ObjectWithPositionDTO(GameObject gate, Vector3 cameraPositionToSet)
    {
        this.gameObject = gate;
        this.position = cameraPositionToSet;
    }

    public ObjectWithPositionDTO(GameObject gameObject, Vector3 position, Quaternion rotation)
        : this(gameObject, position)
    {
        this.rotation = rotation;
    }
}
