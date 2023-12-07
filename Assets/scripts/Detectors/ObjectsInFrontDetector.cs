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

    private EventQueue eventQueue;

    public WallType detectedWallType { get; private set; }

    public Vector3 directionFromPlayerToWall { get; private set; }

    public Vector3 ledgePosition { get; private set; }

    private float minDistanceToClimbWall = 1f;

    public float distanceToCollider { get; set; }

    public Collider wallCollider { get; private set; }

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
        detectedWallType = WallType.NO_WALL;
    }

    private void Update()
    {
        RaycastHit feetHit = DoRaycast(feetLevel);
        RaycastHit midHit = DoRaycast(midLevel);
        RaycastHit headHit = DoRaycast(headLevel);

        if (feetHit.collider != null && midHit.collider == null && headHit.collider == null)
        {
            detectedWallType = WallType.BELOW_HIPS;
        }
        else if (feetHit.collider != null && midHit.collider != null && headHit.collider == null)
        {
            detectedWallType = WallType.ABOVE_HIPS;
            directionFromPlayerToWall = -midHit.normal;
            wallCollider = midHit.collider;
            distanceToCollider = midHit.distance;
        }
        else if (feetHit.collider != null && midHit.collider != null && headHit.collider != null)
        {
            detectedWallType = WallType.ABOVE_HEAD;
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
            if (closerObject.GetComponent<NavMeshAgent>() == null)
            {
                eventQueue.SubmitEvent(new EventDTO(EventType.OBJECT_NOW_IN_RANGE, closerObject));
            }
            else
            {
                detectedWallType = WallType.NO_WALL;
            }
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
