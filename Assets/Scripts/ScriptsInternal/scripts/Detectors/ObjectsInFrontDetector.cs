using UnityEngine;

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

    public bool obstacleFoundInFrontOfCamera { get; private set; }

    private EventQueue eventQueue;

    public bool isCollidingWithGround;

    [SerializeField]
    private float heightAdjustmentForCrouch;

    [SerializeField]
    private float verticalDetectorHeight;

    [SerializeField]
    private float verticalDetectorMaxDistance;

    [SerializeField]
    private float horizontalDetectorHeight;

    [SerializeField]
    private float midClimbBoundaryDistance;

    [SerializeField]
    private float climbBoundaryDistance;

    [SerializeField]
    private float stepBoundaryDistance;

    [SerializeField]
    private float forwardOffsetHorizontalDetector;

    [SerializeField]
    private float horizontalDetectorMaxDistance;

    [SerializeField]
    private float forwardOffsetVerticalDetector;

    [SerializeField]
    private float groundDetectorHeight;

    [SerializeField]
    private float groundDetectorMaxDistanceToGround;

    [SerializeField]
    private float groundDetectorCollisionMaxDistance;

    [SerializeField]
    private float slopeMinDistanceToDetector;

    private bool adjustHeightForCrouch;

    private void Start()
    {
        detectedWallType = WallType.NO_WALL;
        eventQueue = FindAnyObjectByType<EventQueue>();
        isCollidingWithGround = false;
    }

    private void FixedUpdate()
    {
        detectedWallType = WallType.NO_WALL;
        DetectGround();
        DetectObjectsBehind();
        DetectObjectsInFrontAndLedges();
    }

    public void SetIsCrouching(bool crouching)
    {
        adjustHeightForCrouch = crouching;
    }

    private void DetectLedges(RaycastHit objectsInFrontVerticalDetector)
    {
        float distanceToCollision = objectsInFrontVerticalDetector.distance;
        if (
            objectsInFrontVerticalDetector.collider != null
            && distanceToCollision < climbBoundaryDistance
        )
        {
            detectedWallType = WallType.ABOVE_HEAD;
            detectedObject = objectsInFrontVerticalDetector.collider.gameObject;
            verticalCollisionPosition = objectsInFrontVerticalDetector.point;
            Vector3 collisionPoint = objectsInFrontVerticalDetector.point;
            Vector3 playerPositionSameHeight = new Vector3(transform.position.x, collisionPoint.y, transform.position.z);
            Vector3 directionFromPlayerToWall = collisionPoint - playerPositionSameHeight;
            directionFromPlayerToWall.y = 0;
            horizontalCollisionPosition = collisionPoint;
            this.directionFromPlayerToWall = directionFromPlayerToWall;
        }
    }

    private void DetectObjectsInFront(
        RaycastHit objectsInFrontVerticalDetector,
        RaycastHit objectsInFrontHorizontalDetector
    )
    {
        if (objectsInFrontVerticalDetector.collider != null)
        {
            float distanceToCollision = objectsInFrontVerticalDetector.distance;
            if (
                distanceToCollision >= slopeMinDistanceToDetector
                && Vector3.Angle(objectsInFrontVerticalDetector.normal, transform.up) > 2
            )
            {
                obstacleFoundInFrontOfCamera = false;
            }
            else
            {
                obstacleFoundInFrontOfCamera = true;
                detectedObject = objectsInFrontVerticalDetector.collider.gameObject;
                verticalCollisionPosition = objectsInFrontVerticalDetector.point;
            }

            if (
                distanceToCollision >= midClimbBoundaryDistance
                && distanceToCollision < stepBoundaryDistance
            )
            {
                detectedWallType = WallType.BELOW_HIPS;
            }
            else if (
                distanceToCollision < midClimbBoundaryDistance
                && distanceToCollision >= climbBoundaryDistance
            )
            {
                detectedWallType = WallType.ABOVE_HIPS;
            }
        }
        if (objectsInFrontHorizontalDetector.collider != null)
        {
            obstacleFoundInFrontOfCamera = true;
            detectedObject = objectsInFrontHorizontalDetector.collider.gameObject;
        }
        else if (objectsInFrontVerticalDetector.collider == null)
        {
            obstacleFoundInFrontOfCamera = false;
        }
    }

    private float CrouchingAdjustment()
    {
        return (adjustHeightForCrouch ? heightAdjustmentForCrouch : 0);
    }

    private void DetectObjectsInFrontAndLedges()
    {
        RaycastHit objectsInFrontVerticalDetector = CastRayVertical(
            verticalDetectorHeight - CrouchingAdjustment(),
            false,
            verticalDetectorMaxDistance - CrouchingAdjustment(),
            forwardOffsetVerticalDetector
        );

        RaycastHit objectsInFrontHorizontalDetector = CastRayHorizontal(
            horizontalDetectorHeight - CrouchingAdjustment(),
            horizontalDetectorMaxDistance,
            false,
            forwardOffsetHorizontalDetector,
            false
        );
        if (isCollidingWithGround)
        {
            DetectObjectsInFront(objectsInFrontVerticalDetector, objectsInFrontHorizontalDetector);
        }
        else
        {
            DetectLedges(objectsInFrontVerticalDetector);
        }
    }

    private void DetectObjectsBehind()
    {
        RaycastHit objectsBehindHorizontalDetector = CastRayHorizontal(
            horizontalDetectorHeight - CrouchingAdjustment(),
            horizontalDetectorMaxDistance,
            false,
            -forwardOffsetHorizontalDetector,
            true
        );

        RaycastHit objectsBehindVerticalDetector = CastRayVertical(
            verticalDetectorHeight - CrouchingAdjustment(),
            false,
            verticalDetectorMaxDistance - CrouchingAdjustment(),
            -forwardOffsetVerticalDetector
        );

        if (
            objectsBehindVerticalDetector.distance >= slopeMinDistanceToDetector
            && Vector3.Angle(objectsBehindVerticalDetector.normal, transform.up) > 2
        )
        {
            obstacleBehindPlayerDetected = false;
        }
        else if (
            objectsBehindVerticalDetector.collider != null
            || objectsBehindHorizontalDetector.collider != null
        )
        {
            obstacleBehindPlayerDetected = true;
        }
        else
        {
            obstacleBehindPlayerDetected = false;
        }
    }

    private void DetectGround()
    {
        RaycastHit groundHit = CastRayVertical(
            groundDetectorHeight,
            true,
            groundDetectorCollisionMaxDistance
        );
        if (
            groundHit.collider != null
            && !isCollidingWithGround
            && groundHit.distance <= groundDetectorMaxDistanceToGround
        )
        {
            isCollidingWithGround = true;
            detectedObject = groundHit.collider.gameObject;
            eventQueue.SubmitEvent(new EventDTO(EventType.GROUND_DETECTED, null));
        }

        if (
            groundHit.collider == null || groundHit.distance > 2 * groundDetectorMaxDistanceToGround
        )
        {
            if (isCollidingWithGround)
            {
                eventQueue.SubmitEvent(new EventDTO(EventType.STARTED_FALLING, null));
            }
            isCollidingWithGround = false;
        }
    }

    private RaycastHit CastRayHorizontal(
        float height,
        float maxDistance,
        bool debug,
        float forwardOffset = 0,
        bool doBackwardRay = false
    )
    {
        RaycastHit raycastHit;
        Vector3 playerPosition = transform.position;
        Physics.Raycast(
            playerPosition + transform.up * height + transform.forward * forwardOffset,
            (doBackwardRay ? -1 : 1) * transform.forward,
            out raycastHit,
            maxDistance,
            ~0,
            QueryTriggerInteraction.Ignore
        );
        if (debug)
        {
            Debug.DrawRay(
                playerPosition + transform.up * height + transform.forward * forwardOffset,
                (doBackwardRay ? -1 : 1) * transform.forward
            );
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
            transform.position + transform.forward * forwardOffset + transform.up * height;
        Physics.Raycast(
            originPosition,
            transform.up * -1,
            out result,
            maxDistance,
            ~0,
            QueryTriggerInteraction.Ignore
        );
        if (debug)
        {
            Debug.DrawRay(originPosition, transform.up * -1, Color.red);
        }
        return result;
    }
}
