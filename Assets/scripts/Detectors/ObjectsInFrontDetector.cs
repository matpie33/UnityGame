using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ObjectsInFrontDetector : MonoBehaviour
{
    public float minDistanceToInteract;

    public WallType detectedWallType { get; private set; }

    public Vector3 directionFromPlayerToWall { get; private set; }

    public Vector3 ledgePosition { get; private set; }

    public float horizontalDistanceToCollider { get; set; }
    public float verticalDistanceToCollider { get; set; }

    public Vector3 verticalCollisionPosition { get; private set; }

    public Collider wallCollider { get; private set; }

    public GameObject detectedObject { get; private set; }

    [SerializeField]
    private float forwardPositionModifier;

    [SerializeField]
    private float upModifierHighestPoint;

    [SerializeField]
    private float upModifierLowestPoint;

    [SerializeField]
    private float length;

    [SerializeField]
    private float offsetFromPlayerCenter;

    private void Start()
    {
        detectedWallType = WallType.NO_WALL;
    }

    private void FixedUpdate()
    {
        Vector3 pointAboveHead =
            transform.position
            + transform.forward * forwardPositionModifier
            + Vector3.up * upModifierHighestPoint;
        RaycastHit raycastHitVertical;
        float halfHeight = GetComponent<Collider>().bounds.extents.y;
        float playerCenterY = halfHeight + transform.position.y;
        Physics.Raycast(pointAboveHead, Vector3.up * -1, out raycastHitVertical, halfHeight * 2.1f);

        if (raycastHitVertical.collider != null)
        {
            wallCollider = raycastHitVertical.collider;
            verticalDistanceToCollider = raycastHitVertical.distance;

            float raycastYValue = raycastHitVertical.point.y;
            if (raycastYValue > playerCenterY + offsetFromPlayerCenter)
            {
                detectedWallType = WallType.ABOVE_HEAD;
            }
            else if (
                raycastYValue > playerCenterY
                && raycastYValue < playerCenterY + offsetFromPlayerCenter
            )
            {
                detectedWallType = WallType.ABOVE_HIPS;
            }
            else
            {
                detectedWallType = WallType.BELOW_HIPS;
            }
            detectedObject = raycastHitVertical.collider.gameObject;
            verticalCollisionPosition = raycastHitVertical.point;

            RaycastHit raycastHitHorizontal;

            Physics.Raycast(transform.position, transform.forward, out raycastHitHorizontal, 2);
            directionFromPlayerToWall = -raycastHitHorizontal.normal;
            horizontalDistanceToCollider = raycastHitHorizontal.distance;
        }
        else
        {
            detectedWallType = WallType.NO_WALL;
        }
    }
}
