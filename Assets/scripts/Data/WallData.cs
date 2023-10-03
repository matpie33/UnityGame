using UnityEditor;
using UnityEngine;

public class WallData
{
    public Collider wallCollider { get; set; }

    public Vector3 directionFromPlayerToWall { get; set; }

    public float distanceToCollider { get; set; }
}
