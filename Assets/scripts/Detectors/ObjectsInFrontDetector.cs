using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectsInFrontDetector : Publisher
{
    public float minDistanceToPickup = 3;

    [SerializeField]
    private Transform feetLevel;

    [SerializeField]
    private Transform midLevel;

    [SerializeField]
    private Transform headLevel;

    [SerializeField]
    private GameObject pickupHint;

    private PickupObjectsController pickupObjectsController;

    public WallType detectedWallType { get; private set; }

    private const float minDistanceToClimbWall = 0.3f;

    private void Start()
    {
        detectedWallType = WallType.NO_WALL;
        pickupObjectsController = GetComponent<PickupObjectsController>();
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
        }
        else if (feetHit.collider != null && midHit.collider != null && headHit.collider != null)
        {
            detectedWallType = WallType.ABOVE_HEAD;
        }
        else
        {
            detectedWallType = WallType.NO_WALL;
        }

        if (feetHit.collider != null)
        {
            float distance = feetHit.distance;
            if (feetHit.collider.tag.Equals(Tags.PICKABLE) && distance < minDistanceToPickup)
            {
                pickupHint.SetActive(true);
                pickupObjectsController.objectInFront = feetHit.collider.gameObject;
            }
            else
            {
                pickupHint.SetActive(false);
                pickupObjectsController.objectInFront = null;
            }
        }
        else
        {
            pickupHint.SetActive(false);
            pickupObjectsController.objectInFront = null;
        }
    }

    private RaycastHit DoRaycast(Transform transform)
    {
        Vector3 position = transform.position;
        RaycastHit raycastHit;
        Physics.Raycast(position, this.transform.forward, out raycastHit, minDistanceToClimbWall);

        return raycastHit;
    }
}
