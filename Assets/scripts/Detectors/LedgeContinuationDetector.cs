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

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public bool CheckIfThereIsSpaceForGrab(LedgeDirection ledgeDirection)
    {
        bool isLeftSide = ledgeDirection.Equals(LedgeDirection.LEFT);

        Vector3 direction = characterController.transform.forward;

        Vector3 abovePoint = getAboveCheckPoint(isLeftSide);

        bool aboveCheckHit = Physics.Raycast(abovePoint, direction, 1f);

        return !aboveCheckHit;
    }

    public bool CheckIfLedgeContinues(LedgeDirection ledgeDirection)
    {
        bool isLeftSide = ledgeDirection.Equals(LedgeDirection.LEFT);

        Vector3 origin = getHorizontalCheckPoint(isLeftSide);
        Vector3 direction = characterController.transform.forward;

        bool horizontalCheckHit = Physics.Raycast(origin, direction, 1f);

        return horizontalCheckHit;
    }

    public bool CheckIfCanWalkDownLedge()
    {
        return false;
    }

    internal bool CheckIfCanRotateAroundLedge(LedgeDirection ledgeDirection)
    {
        bool isLeftSide = ledgeDirection.Equals(LedgeDirection.LEFT);
        Vector3 origin = getHorizontalCheckPoint(isLeftSide);

        Vector3 directionEndPoint =
            origin
            + characterController.transform.forward * 2
            - Vector3.up
            + (isLeftSide ? -1 : 1) * characterController.transform.right * 0.8f;

        Vector3 direction = directionEndPoint - origin;

        return !Physics.Raycast(origin, direction, 2f);
    }

    private Vector3 getHorizontalCheckPoint(bool isLeftSide)
    {
        return (isLeftSide ? leftHand : rightHand).transform.position
            + (isLeftSide ? -1 : 1) * 0.05f * characterController.transform.right
            - Vector3.up * 0.1f
            - characterController.transform.forward * 0.2f;
    }

    private Vector3 getAboveCheckPoint(bool isLeftSide)
    {
        return (isLeftSide ? leftHand : rightHand).transform.position
            + (isLeftSide ? -1 : 1) * 0.05f * characterController.transform.right
            + Vector3.up * 0.15f
            - characterController.transform.forward * 0.2f;
    }

    internal Vector3 GetRotatedVector(Vector3 directionFromWallToPlayer, LedgeDirection direction)
    {
        bool isLeftSide = direction.Equals(LedgeDirection.LEFT);

        Quaternion rotation = Quaternion.Euler(new Vector3(0, isLeftSide ? 90 : -90, 0));
        return rotation * directionFromWallToPlayer;
    }
}
