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

    public Vector3 verticalCollisionPosition { get; private set; }
    public Vector3 horizontalCollisionPosition { get; private set; }

    public Collider wallCollider { get; private set; }

    public GameObject detectedObject { get; private set; }

    [SerializeField]
    private float forwardOffsetFromPlayer;

    [SerializeField]
    private float minHeightToStep;

    [SerializeField]
    private float minHeightToClimb;

    [SerializeField]
    private float maxDistanceToWallStep;

    [SerializeField]
    private float maxDistanceToWallClimb;

    [SerializeField]
    private float offsetYForFindingLedge;

    [SerializeField]
    private float maxDistanceToWallHorizontal;

    [SerializeField]
    private float maxSlopeAngle;

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
        RaycastHit feetLevelHit = CastRayHorizontal(0);

        float angle = Vector3.Angle(Vector3.up, feetLevelHit.normal);
        if (angle > 0 && angle < maxSlopeAngle)
        {
            detectedWallType = WallType.NO_WALL;
            obstacleFoundInFrontOfCamera = false;
            return;
        }

        RaycastHit grabLevelCheckLow = CastRayHorizontal(minHeightToClimb);
        RaycastHit grabLevelCheckHigher = CastRayHorizontal(
            minHeightToClimb + offsetYForFindingLedge
        );
        RaycastHit stepLevelHit = CastRay(minHeightToStep, false, maxDistanceToWallStep);
        RaycastHit climbLevelHit = CastRay(minHeightToClimb, true, maxDistanceToWallClimb);

        if (
            feetLevelHit.collider == null
            && stepLevelHit.collider == null
            && climbLevelHit.collider == null
        )
        {
            obstacleFoundInFrontOfCamera = false;
            detectedWallType = WallType.NO_WALL;
        }

        if (feetLevelHit.collider != null)
        {
            obstacleFoundInFrontOfCamera = true;
        }

        if (feetLevelHit.collider != null && stepLevelHit.collider != null)
        {
            detectedWallType = WallType.BELOW_HIPS;
            obstacleFoundInFrontOfCamera = true;
            detectedObject = stepLevelHit.collider.gameObject;
            verticalCollisionPosition = stepLevelHit.point;
        }

        if (feetLevelHit.collider != null && climbLevelHit.collider != null)
        {
            detectedWallType = WallType.ABOVE_HIPS;
            obstacleFoundInFrontOfCamera = true;
            detectedObject = climbLevelHit.collider.gameObject;
            verticalCollisionPosition = climbLevelHit.point;
        }
        if (grabLevelCheckLow.collider != null && grabLevelCheckHigher.collider == null)
        {
            detectedWallType = WallType.ABOVE_HEAD;
            obstacleFoundInFrontOfCamera = true;
            detectedObject = grabLevelCheckLow.collider.gameObject;
            verticalCollisionPosition = grabLevelCheckLow.point;
            horizontalCollisionPosition = grabLevelCheckLow.point;
            directionFromPlayerToWall = -grabLevelCheckLow.normal;
        }

        RaycastHit raycastHitBehind;
        Physics.Raycast(
            transform.position,
            -transform.forward,
            out raycastHitBehind,
            maxDistanceToWallHorizontal
        );

        if (raycastHitBehind.collider != null)
        {
            obstacleBehindPlayerDetected = true;
        }
        else
        {
            obstacleBehindPlayerDetected = false;
        }
    }

    private RaycastHit CastRayHorizontal(float height)
    {
        RaycastHit raycastHit;
        Vector3 playerPosition = transform.position;
        Physics.Raycast(
            playerPosition + Vector3.up * height,
            transform.forward,
            out raycastHit,
            maxDistanceToWallHorizontal
        );
        return raycastHit;
    }

    private RaycastHit CastRay(float height, bool debug, float maxDistance)
    {
        RaycastHit result;
        Vector3 feetLevelPosition =
            transform.position + transform.forward * forwardOffsetFromPlayer + Vector3.up * height;
        Physics.Raycast(feetLevelPosition, Vector3.up * -1, out result, maxDistance);
        if (debug)
        {
            Debug.DrawRay(feetLevelPosition, Vector3.up * -1);
        }
        return result;
    }
}
