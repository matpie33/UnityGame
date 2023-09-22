using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectsInFrontDetector : MonoBehaviour
{
    public float minDistanceToInteract = 3;

    [SerializeField]
    private Transform feetLevel;

    [SerializeField]
    private Transform midLevel;

    [SerializeField]
    private Transform headLevel;

    private EventQueue eventQueue;

    public WallType detectedWallType { get; private set; }

    public Vector3 directionFromPlayerToWall { get; private set; }

    private const float minDistanceToClimbWall = 1f;

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
            directionFromPlayerToWall = midHit.normal;
        }
        else if (feetHit.collider != null && midHit.collider != null && headHit.collider != null)
        {
            detectedWallType = WallType.ABOVE_HEAD;
        }
        else
        {
            detectedWallType = WallType.NO_WALL;
        }

        bool detectedCollision = DetectCollision(feetHit);
        if (!detectedCollision)
        {
            DetectCollision(midHit);
        }
    }

    private bool DetectCollision(RaycastHit raycastHit)
    {
        bool detectedCollision = false;
        if (raycastHit.collider != null)
        {
            float distance = raycastHit.distance;
            if (
                raycastHit.collider.tag.Equals(Tags.INTERACTABLE)
                && distance < minDistanceToInteract
            )
            {
                detectedCollision = true;
                eventQueue.SubmitEvent(
                    new EventDTO(
                        EventType.OBJECT_NOW_IN_RANGE,
                        raycastHit.collider.gameObject.GetComponent<Interactable>()
                    )
                );
            }
            else
            {
                eventQueue.SubmitEvent((new EventDTO(EventType.OBJECT_OUT_OF_RANGE, null)));
            }
        }
        else
        {
            eventQueue.SubmitEvent((new EventDTO(EventType.OBJECT_OUT_OF_RANGE, null)));
        }
        return detectedCollision;
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

        return raycastHit;
    }
}
