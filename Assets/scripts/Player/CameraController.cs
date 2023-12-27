using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : Observer
{
    [Header("Framing")]
    [SerializeField]
    private new Camera camera = null;

    [SerializeField]
    private Transform playerCenterPoint = null;

    private Vector3 planarDirection; //camera's forward on xz plane
    private Quaternion targetRotation;

    [Header("Rotation")]
    [SerializeField]
    [Range(-90, 90)]
    private float minVerticalAngle = -90;

    [Range(-90, 90)]
    [SerializeField]
    private float maxVerticalAngle = 90;

    [Header("Distance")]
    [SerializeField]
    private float zoomSpeed = 10f;

    [SerializeField]
    private float rotationSharpness = 25;

    [SerializeField]
    private float minDistance;

    [SerializeField]
    private float maxDistance;

    [SerializeField]
    private float defaultDistance = 2f;

    [SerializeField]
    private LayerMask obstructionLayers;

    private List<Collider> ignoredColliders = new List<Collider>();

    private float targetVerticalAngle;

    private Vector3 targetPosition;

    private float targetDistance;

    public Vector3 cameraPlanarDirection
    {
        get => planarDirection;
    }

    private Vector3 playerPositionOffset;

    [SerializeField]
    private bool invertX;

    [SerializeField]
    private bool invertY;

    [SerializeField]
    private float defaultVerticalAngle = 20f;

    private Vector3 newPosition;

    private Quaternion newRotation;

    private float modifiedDistance;

    private void OnValidate()
    {
        defaultDistance = Mathf.Clamp(defaultDistance, minDistance, maxDistance);
        defaultVerticalAngle = Mathf.Clamp(
            defaultVerticalAngle,
            minVerticalAngle,
            maxVerticalAngle
        );
    }

    public void adjustCameraForCrouch()
    {
        playerPositionOffset.x = -0.02f;
        playerPositionOffset.y = -0.69f;
    }

    public void adjustCameraForStanding()
    {
        playerPositionOffset.x = 0;
        playerPositionOffset.y = 0;
    }

    private void Start()
    {
        obstructionLayers = 1 << 2;
        obstructionLayers = ~obstructionLayers;
        playerPositionOffset = new Vector3(0, 0, 0);
        ignoredColliders.AddRange(GetComponentsInChildren<Collider>());
        planarDirection = playerCenterPoint.forward;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        targetDistance = defaultDistance;
        targetVerticalAngle = defaultVerticalAngle;
        targetRotation =
            Quaternion.LookRotation(planarDirection) * Quaternion.Euler(targetVerticalAngle, 0, 0);
        targetPosition =
            playerCenterPoint.position - (targetRotation * Vector3.forward) * targetDistance;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        bool didHit = Physics.Raycast(
            playerCenterPoint.position,
            -camera.transform.forward,
            out hit,
            targetDistance,
            obstructionLayers
        );

        if (didHit && hit.collider.gameObject != gameObject)
        {
            modifiedDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            modifiedDistance = Mathf.Infinity;
        }
    }

    private void LateUpdate()
    {
        float zoom = -PlayerInputs.MouseScrollInput * zoomSpeed;
        float mouseX = PlayerInputs.MouseXInput;
        float mouseY = -PlayerInputs.MouseYInput;
        if (invertX)
        {
            mouseX *= -1;
        }
        if (invertY)
        {
            mouseY *= -1;
        }

        Vector3 focusPosition =
            playerCenterPoint.position
            + new Vector3(playerPositionOffset.x, playerPositionOffset.y, 0);

        planarDirection = Quaternion.Euler(0, mouseX, 0) * planarDirection; //rotate planar direction (mouseX degrees) around y axis
        targetDistance = Mathf.Clamp(targetDistance + zoom, minDistance, maxDistance);
        targetVerticalAngle = Mathf.Clamp(
            targetVerticalAngle + mouseY,
            minVerticalAngle,
            maxVerticalAngle
        );

        float smallestDistance = Math.Min(targetDistance, modifiedDistance);

        targetRotation =
            Quaternion.LookRotation(planarDirection) * Quaternion.Euler(targetVerticalAngle, 0, 0);
        targetPosition = focusPosition - (targetRotation * Vector3.forward) * (smallestDistance);
        newRotation = Quaternion.Lerp(
            camera.transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSharpness
        );
        newPosition = Vector3.Lerp(
            camera.transform.position,
            targetPosition,
            Time.deltaTime * rotationSharpness
        );

        camera.transform.position = newPosition;
        camera.transform.rotation = newRotation;
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.LEVER_OPENING:
            case EventType.QUEST_CONFIRMATION_NEEDED:
                enabled = false;
                break;
            case EventType.GATE_OPENED:
            case EventType.QUEST_REJECTED:
            case EventType.QUEST_ACCEPTED:
                enabled = true;
                break;
        }
    }
}
