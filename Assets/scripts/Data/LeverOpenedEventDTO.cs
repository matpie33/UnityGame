using UnityEditor;
using UnityEngine;

public class LeverOpenedEventDTO
{
    public GameObject gate { get; private set; }
    public Vector3 cameraPositionToSet { get; private set; }

    public LeverOpenedEventDTO(GameObject gate, Vector3 cameraPositionToSet)
    {
        this.gate = gate;
        this.cameraPositionToSet = cameraPositionToSet;
    }
}
