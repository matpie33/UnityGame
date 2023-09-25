using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeContinuationDetector : MonoBehaviour
{
    [SerializeField]
    private GameObject leftHand;

    [SerializeField]
    private GameObject rightHand;

    private CharacterController characterController;

    [SerializeField]
    private GameObject debug;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public bool CheckIfLedgeContinues(LedgeDirection ledgeDirection)
    {
        Vector3 directionFromWallToPlayer = characterController.wallData.directionFromWallToPlayer;
        Collider wallCollider = characterController.wallData.wallCollider;

        GameObject colliderObject = wallCollider.gameObject;
        Vector3 ledgePosition = colliderObject.transform.position;
        Vector3 colliderExtents = wallCollider.bounds.extents;

        bool isLeftSide = ledgeDirection.Equals(LedgeDirection.LEFT);
        Vector3 handPosition = (isLeftSide ? leftHand : rightHand).transform.position;
        Vector3 directionFromHandToLedge = handPosition - ledgePosition;
        directionFromHandToLedge.y = 0;
        Quaternion rotateRight = Quaternion.Euler(new Vector3(0, 90, 0));
        Vector3 directionAlongTheLedge = rotateRight * directionFromWallToPlayer;
        Vector3 distanceAlongTheLedge = Vector3.Project(
            directionFromHandToLedge,
            directionAlongTheLedge
        );
        bool ledgeContinues =
            distanceAlongTheLedge.magnitude
            <= Vector3.Scale(colliderExtents, directionAlongTheLedge.normalized).magnitude;
        debug.transform.position =
            colliderObject.transform.position + distanceAlongTheLedge + Vector3.up * 2;
        return ledgeContinues;
    }

    internal bool CheckIfCanRotateAroundLedge(LedgeDirection ledgeDirection)
    {
        Vector3 directionFromWallToPlayer = characterController.wallData.directionFromWallToPlayer;
        Collider wallCollider = characterController.wallData.wallCollider;

        GameObject colliderObject = wallCollider.gameObject;
        Vector3 ledgePosition = colliderObject.transform.position;
        Vector3 colliderExtents = wallCollider.bounds.extents;

        bool isLeftSide = ledgeDirection.Equals(LedgeDirection.LEFT);
        Vector3 handPosition = (isLeftSide ? leftHand : rightHand).transform.position;
        Vector3 directionFromHandToLedge = handPosition - ledgePosition;
        directionFromHandToLedge.y = 0;
        Quaternion rotateRight = Quaternion.Euler(new Vector3(0, 90, 0));
        Vector3 directionAlongTheLedge = rotateRight * directionFromWallToPlayer;
        Vector3 distanceAlongTheLedge = Vector3.Project(
            directionFromHandToLedge,
            directionAlongTheLedge
        );

        Vector3 distanceToPlayer = Vector3.Scale(colliderExtents, directionFromWallToPlayer);
        Vector3 distanceToUp = Vector3.Scale(colliderExtents, Vector3.up);

        Vector3 position =
            colliderObject.transform.position
            + distanceAlongTheLedge * 1.04f
            + distanceToUp * 0.96f
            + distanceToPlayer;
        debug.transform.position = position;
        Vector3 directionEndPoint = position + directionFromWallToPlayer + Vector3.up;
        Vector3 direction = position - directionEndPoint;
        return !Physics.Raycast(position, direction, 2f);
    }
}
