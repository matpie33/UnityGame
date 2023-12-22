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

    [SerializeField]
    private Transform feetLevel;

    [SerializeField]
    private Transform midLevel;

    [SerializeField]
    private Transform headLevel;

    [SerializeField]
    private Transform aboveHeadLevel;

    private EventQueue eventQueue;

    public WallType detectedWallType { get; private set; }

    public Vector3 directionFromPlayerToWall { get; private set; }

    public Vector3 ledgePosition { get; private set; }

    private float minDistanceToClimbWall = 1f;

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

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
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
        Physics.Raycast(pointAboveHead, Vector3.up * -1, out raycastHitVertical, halfHeight * 2);

        if (raycastHitVertical.collider != null)
        {
            wallCollider = raycastHitVertical.collider;
            verticalDistanceToCollider = raycastHitVertical.distance;
            if (raycastHitVertical.point.y > playerCenterY)
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
            return;
        }
        else
        {
            detectedWallType = WallType.NO_WALL;
        }

        Vector3 originForStepUp =
            transform.position
            + transform.forward * forwardPositionModifier
            + Vector3.up * upModifierLowestPoint;
        RaycastHit resultForStepUp;
        //Physics.Raycast(originForStepUp, Vector3.up * -1, out resultForStepUp, length);

        //Debug.DrawRay(originForStepUp, Vector3.up * -1);

        //if (resultForStepUp.collider != null)
        //{
        //    detectedWallType = WallType.BELOW_HIPS;
        //    directionFromPlayerToWall = resultForAboveHeadLedge.point - transform.position;
        //    directionFromPlayerToWall = new Vector3(
        //        directionFromPlayerToWall.x,
        //        0,
        //        directionFromPlayerToWall.z
        //    );
        //    Debug.Log("above hips");

        //    wallCollider = resultForAboveHeadLedge.collider;
        //    distanceToCollider = directionFromPlayerToWall.magnitude;
        //    detectedObject = resultForAboveHeadLedge.collider.gameObject;

        //    return;
        //}

        if (true)
        {
            return;
        }

        RaycastHit feetHit = DoRaycast(feetLevel);
        RaycastHit midHit = DoRaycast(midLevel);
        RaycastHit headHit = DoRaycast(headLevel);
        RaycastHit aboveHeadHit = DoRaycast(aboveHeadLevel);

        if (feetHit.collider != null && midHit.collider == null && headHit.collider == null)
        {
            detectedWallType = WallType.BELOW_HIPS;
        }
        else if (feetHit.collider != null && midHit.collider != null && headHit.collider == null)
        {
            detectedWallType = WallType.ABOVE_HIPS;
            directionFromPlayerToWall = -midHit.normal;
            wallCollider = midHit.collider;
            horizontalDistanceToCollider = midHit.distance;
            detectedObject = midHit.collider.gameObject;
        }
        else if (
            feetHit.collider != null
            && midHit.collider != null
            && headHit.collider != null
            && aboveHeadHit.collider == null
        )
        {
            detectedWallType = WallType.ABOVE_HEAD;
            directionFromPlayerToWall = -headHit.normal;
            wallCollider = headHit.collider;
            horizontalDistanceToCollider = headHit.distance;
            detectedObject = headHit.collider.gameObject;
        }
        else
        {
            detectedWallType = WallType.NO_WALL;
        }

        GetCloserCollision(feetHit, midHit);
    }

    private void GetCloserCollision(RaycastHit feetHit, RaycastHit midHit)
    {
        float smallerDistance = Mathf.Infinity;
        GameObject closerObject = null;
        CheckCollision(feetHit, ref smallerDistance, ref closerObject);
        CheckCollision(midHit, ref smallerDistance, ref closerObject);

        if (closerObject != null)
        {
            detectedObject = closerObject;
            eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_NOW_IN_RANGE, closerObject));
        }
        else
        {
            eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_OUT_OF_RANGE, null));
        }
    }

    private void CheckCollision(
        RaycastHit raycastHit,
        ref float smallestDistance,
        ref GameObject closestObject
    )
    {
        Collider collider = raycastHit.collider;
        if (collider != null)
        {
            float distance = raycastHit.distance;
            if (distance < minDistanceToInteract && distance < smallestDistance)
            {
                smallestDistance = distance;
                closestObject = collider.gameObject;
            }
        }
    }

    private RaycastHit DoRaycast(Transform transform)
    {
        Vector3 position = transform.position;
        RaycastHit raycastHit;
        int layerMask = 1 << 2;
        layerMask = ~layerMask;

        Physics.Raycast(
            position,
            this.transform.forward,
            out raycastHit,
            minDistanceToClimbWall,
            layerMask
        );
        Vector3 vector3 = this.transform.forward * minDistanceToClimbWall;

        return raycastHit;
    }
}
