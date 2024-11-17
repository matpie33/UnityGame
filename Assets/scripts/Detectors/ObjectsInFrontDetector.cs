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
    private float forwardOffsetFromPlayerGrabLevel;

    [SerializeField]
    private float minHeightToStep;

    [SerializeField]
    private float minHeightToClimb;

    [SerializeField]
    private float minHeightToGrab;

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

    [SerializeField]
    private float groundCheckerForwardOffset;

    [SerializeField]
    private float maxDistanceToGrabVertically;

    public bool obstacleFoundInFrontOfCamera { get; private set; }

    private EventQueue eventQueue;

    public bool isCollidingWithGround;

    private void Start()
    {
        detectedWallType = WallType.NO_WALL;
        eventQueue = FindAnyObjectByType<EventQueue>();
        isCollidingWithGround = false;
    }

    private void FixedUpdate()
    {
        DetectWallsInForwardDirection();
    }

    private void DetectWallsInForwardDirection()
    {
        //TODO make it possible to pass max distance to CastRayHorizontal, maybe merge these 2 methods
        RaycastHit feetLevelHit = CastRayHorizontal(0, false);
        RaycastHit headLevelHit = CastRayHorizontal(minHeightToClimb, false);

        WallType currentWallType = WallType.NOT_REACHABLE;
        if (feetLevelHit.collider != null)
        {
            detectedObject = feetLevelHit.collider.gameObject;
        }

        float angle = Vector3.Angle(Vector3.up, feetLevelHit.normal);
        if (angle > 0 && angle < maxSlopeAngle)
        {
            detectedWallType = WallType.NO_WALL;
            obstacleFoundInFrontOfCamera = false;
            return;
        }

        RaycastHit grabLevelHit = CastRayVertical(
            minHeightToGrab,
            false,
            maxDistanceToGrabVertically,
            forwardOffsetFromPlayerGrabLevel
        );

        RaycastHit groundHit = CastRayVertical(0.1f, false, .2f, groundCheckerForwardOffset);

        RaycastHit stepLevelHit = CastRayVertical(minHeightToStep, false, maxDistanceToWallStep);
        RaycastHit climbLevelHit = CastRayVertical(minHeightToClimb, false, maxDistanceToWallClimb);

        if (groundHit.collider != null && !isCollidingWithGround)
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.GROUND_DETECTED, null));
            isCollidingWithGround = true;
        }
        if (groundHit.collider == null)
        {
            isCollidingWithGround = false;
        }

        if (
            feetLevelHit.collider == null
            && stepLevelHit.collider == null
            && climbLevelHit.collider == null
        )
        {
            obstacleFoundInFrontOfCamera = false;
            currentWallType = WallType.NO_WALL;
        }

        if (feetLevelHit.collider != null && stepLevelHit.collider != null)
        {
            currentWallType = WallType.BELOW_HIPS;
            obstacleFoundInFrontOfCamera = true;
            detectedObject = stepLevelHit.collider.gameObject;
            verticalCollisionPosition = stepLevelHit.point;
        }

        if (
            feetLevelHit.collider != null
            && climbLevelHit.collider != null
            && headLevelHit.collider == null
        )
        {
            currentWallType = WallType.ABOVE_HIPS;
            obstacleFoundInFrontOfCamera = true;
            detectedObject = climbLevelHit.collider.gameObject;
            verticalCollisionPosition = climbLevelHit.point;
        }
        if (grabLevelHit.collider != null)
        {
            currentWallType = WallType.ABOVE_HEAD;
            obstacleFoundInFrontOfCamera = true;
            detectedObject = grabLevelHit.collider.gameObject;
            verticalCollisionPosition = grabLevelHit.point;
            horizontalCollisionPosition = feetLevelHit.point;
            directionFromPlayerToWall = -feetLevelHit.normal;
        }
        if (feetLevelHit.collider != null && headLevelHit.collider != null)
        {
            obstacleFoundInFrontOfCamera = true;
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
        detectedWallType = currentWallType;
    }

    private RaycastHit CastRayHorizontal(float height, Boolean debug)
    {
        RaycastHit raycastHit;
        Vector3 playerPosition = transform.position;
        Physics.Raycast(
            playerPosition + transform.up * height,
            transform.forward,
            out raycastHit,
            maxDistanceToWallHorizontal
        );
        if (debug)
        {
            Debug.DrawRay(playerPosition + transform.up * height, transform.forward);
        }
        return raycastHit;
    }

    private RaycastHit CastRayVertical(
        float height,
        bool debug,
        float maxDistance,
        float forwardOffset = 0
    )
    {
        RaycastHit result;
        Vector3 originPosition =
            transform.position
            + transform.forward * (forwardOffset == 0 ? forwardOffsetFromPlayer : forwardOffset)
            + transform.up * height;
        Physics.Raycast(originPosition, transform.up * -1, out result, maxDistance);
        if (debug)
        {
            Debug.DrawRay(originPosition, transform.up * -1);
        }
        return result;
    }
}
