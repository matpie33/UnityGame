using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeContinuationDetector : MonoBehaviour
{
    [SerializeField]
    private GameObject leftSideDetector;

    [SerializeField]
    private GameObject rightSideDetector;

    private CharacterController characterController;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public bool CheckIfLedgeContinues(LedgeDirection ledgeDirection)
    {
        RaycastHit firstHit;
        RaycastHit secondHit;
        Vector3 rightSidePosition = rightSideDetector.transform.position;
        Vector3 leftSidePosition = leftSideDetector.transform.position;
        float distanceBetweenHands = Vector3.Distance(rightSidePosition, leftSidePosition);
        bool isLeftSide = ledgeDirection.Equals(LedgeDirection.LEFT);

        bool didHitFirst = Physics.Raycast(
            isLeftSide ? leftSidePosition : rightSidePosition,
            characterController.transform.forward,
            out firstHit,
            2f
        );
        bool didHitSecond = Physics.Raycast(
            (isLeftSide ? leftSidePosition : rightSidePosition)
                + (isLeftSide ? -1 : 1)
                    * characterController.transform.right
                    * distanceBetweenHands,
            characterController.transform.forward,
            out secondHit,
            2f
        );

        return didHitFirst && didHitSecond;
    }
}
