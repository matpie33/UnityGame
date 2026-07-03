using UnityEngine;
using UnityEngine.Serialization;

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
    private float slopeMinDistanceToDetector;

    private bool adjustHeightForCrouch;

    [FormerlySerializedAs("newDetectorHeight")]
    [SerializeField]
    private float ledgeDetectorHeight;

    [FormerlySerializedAs("newDetectorMaxDistance")]
    [SerializeField]
    private float ledgeDetectorMaxDistance;

    [SerializeField]
    private float ledgeDetectorForwardOffset;

    private CharacterController characterController;

    public Vector3 ledgeCollisionPoint;

    [FormerlySerializedAs("aboveHipsModifier")]
    [SerializeField]
    private float aboveHipsOffset;

    [FormerlySerializedAs("aboveHeadModifier")]
    [SerializeField]
    private float aboveHeadOffset;

    private void Start()
    {
        detectedWallType = WallType.NO_WALL;
        characterController = FindAnyObjectByType<CharacterController>();
    }

    private void FixedUpdate()
    {
        detectedWallType = WallType.NO_WALL;
        DetectObjectsBehind();
        DetectObjectsInFrontAndLedges();
    }

    public void SetIsCrouching(bool crouching)
    {
        adjustHeightForCrouch = crouching;
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
        

        DetectObjectsInFront(objectsInFrontVerticalDetector, objectsInFrontHorizontalDetector);

        float height = ledgeDetectorHeight;
        if (!characterController.groundDetector.isCollidingWithGround)
        {
            height -= 2;
        }
        RaycastHit ledgeDetector = CastRayVertical(
            height - CrouchingAdjustment(),
            true,
            ledgeDetectorMaxDistance - CrouchingAdjustment(),
            ledgeDetectorForwardOffset
        );                    
        DetectLedges(ledgeDetector, objectsInFrontVerticalDetector);
    }

    private void DetectLedges(RaycastHit ledgeDetector, RaycastHit objectsInFrontVerticalDetector)
    {
        if (ledgeDetector.collider != null && !ledgeDetector.collider.gameObject.CompareTag(Tags.ENEMY))
        {
            float distanceToCollision = ledgeDetector.distance;
            ledgeCollisionPoint = ledgeDetector.point;

            float collisionYPoint = ledgeCollisionPoint.y;
            var angle = Vector3.Angle(ledgeDetector.normal, transform.up);
            if (angle < .5f || !characterController.groundDetector.isCollidingWithGround)
            {
                if (collisionYPoint > transform.position.y + aboveHeadOffset)
                {

                    detectedWallType = WallType.ABOVE_HEAD;
                }
                else if (collisionYPoint > transform.position.y + aboveHipsOffset)
                {
                    detectedWallType = WallType.ABOVE_HIPS;
                }
                else
                {
                    detectedWallType = WallType.BELOW_HIPS;
                }
            }
           

            detectedObject = ledgeDetector.collider.gameObject;
            verticalCollisionPosition = ledgeDetector.point;
            Vector3 collisionPoint = ledgeDetector.point;
            Vector3 playerPositionSameHeight = new Vector3(transform.position.x, collisionPoint.y, transform.position.z);
            Vector3 directionFromPlayerToWall = collisionPoint - playerPositionSameHeight;
            directionFromPlayerToWall.y = 0;
            horizontalCollisionPosition = collisionPoint;
            this.directionFromPlayerToWall = directionFromPlayerToWall;
        } else
        {
            detectedObject = null;
            detectedWallType = WallType.NO_WALL;
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
            -transform.up,
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
