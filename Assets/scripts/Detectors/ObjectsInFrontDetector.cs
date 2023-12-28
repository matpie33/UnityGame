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

    public bool obstacleBehindPlayerDetected { get; private set; }

    public WallType detectedWallType { get; private set; }

    public Vector3 directionFromPlayerToWall { get; private set; }

    public Vector3 ledgePosition { get; private set; }

    public Vector3 verticalCollisionPosition { get; private set; }
    public Vector3 horizontalCollisionPosition { get; private set; }

    public Collider wallCollider { get; private set; }

    public GameObject detectedObject { get; private set; }

    [SerializeField]
    private float cameraForwardOffset;

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

    public bool obstacleFoundInFrontOfCamera { get; private set; }

    private void Start()
    {
        detectedWallType = WallType.NO_WALL;
    }

    private void FixedUpdate()
    {
        DetectWallsInForwardDirection();
    }

    private void DetectWallsInForwardDirection()
    {
        Vector3 pointAboveHead =
            transform.position
            + transform.forward * forwardPositionModifier
            + Vector3.up * upModifierHighestPoint;
        RaycastHit raycastHitVertical;
        float halfHeight = GetComponent<Collider>().bounds.extents.y;
        float playerCenterY = halfHeight + transform.position.y;
        Physics.Raycast(pointAboveHead, Vector3.up * -1, out raycastHitVertical, halfHeight * 2.1f);

        Vector3 pointBehindPlayer =
            transform.position - transform.forward * .3f + Vector3.up * upModifierHighestPoint;

        RaycastHit raycastHitBehind;
        Physics.Raycast(
            pointBehindPlayer,
            Vector3.up * -1,
            out raycastHitBehind,
            halfHeight * 2.1f
        );

        if (raycastHitBehind.collider != null)
        {
            obstacleBehindPlayerDetected = true;
        }
        else
        {
            obstacleBehindPlayerDetected = false;
        }

        if (raycastHitVertical.collider != null)
        {
            wallCollider = raycastHitVertical.collider;
            obstacleFoundInFrontOfCamera = true;

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

            bool didHit = Physics.Raycast(
                transform.position,
                transform.forward,
                out raycastHitHorizontal,
                2
            );
            if (!didHit)
            {
                didHit = Physics.Raycast(
                    transform.position + halfHeight * Vector3.up,
                    transform.forward,
                    out raycastHitHorizontal,
                    2
                );
            }
            if (didHit)
            {
                horizontalCollisionPosition = raycastHitHorizontal.point;
                directionFromPlayerToWall = -raycastHitHorizontal.normal;
            }
        }
        else
        {
            RaycastHit raycastHitHorizontal;

            Physics.Raycast(
                transform.position + Vector3.up * .3f,
                transform.forward,
                out raycastHitHorizontal,
                .6f
            );
            if (raycastHitHorizontal.collider != null)
            {
                obstacleFoundInFrontOfCamera = true;
            }
            else
            {
                detectedWallType = WallType.NO_WALL;
                obstacleFoundInFrontOfCamera = false;
            }
        }
    }
}
