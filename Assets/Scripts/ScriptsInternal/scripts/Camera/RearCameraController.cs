using System.Collections;
using UnityEngine;

public class RearCameraController : Observer
{
    private GameObject rearCamera;

    private CharacterController characterController;

    [SerializeField]
    private Vector3 cameraAdjustmentVector;

    [SerializeField]
    private float distanceToPlayer;

    [SerializeField]
    private float cameraLifeTime;

    private void Start()
    {
        rearCamera = transform.Find("RearCamera").gameObject;
        characterController = FindAnyObjectByType<CharacterController>();
    }

    private IEnumerator HideAfterSeconds()
    {
        yield return new WaitForSeconds(cameraLifeTime);
        rearCamera.SetActive(false);
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        if (eventDTO.eventType.Equals(EventType.FALLING_BALL_TRIGGERED))
        {
            rearCamera.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideAfterSeconds());
        }
    }

    private void LateUpdate()
    {
        Vector3 normalToGround = characterController.stateMachine.runState.vectorNormalToGround;
        Vector3 parallelToGround = Vector3.ProjectOnPlane(
            characterController.transform.forward,
            normalToGround
        );
        rearCamera.transform.position =
            characterController.transform.position
            - parallelToGround * distanceToPlayer
            + cameraAdjustmentVector;
        rearCamera.transform.LookAt(characterController.transform);
    }
}
