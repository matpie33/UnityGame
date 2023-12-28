using UnityEditor;
using UnityEngine;

public class WallData
{
    public Collider wallCollider { get; set; }

    public Vector3 verticalCollisionPoint { get; set; }
    public Vector3 horizontalCollisionPoint { get; set; }

    public Vector3 directionFromPlayerToWall { get; set; }
}
