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
    private Transform playerTransform = null;

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
    private float minDistance = 0;

    [SerializeField]
    private float maxDistance = 10f;

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

    [SerializeField]
    private Vector3 framing;

    [SerializeField]
    private bool invertX;

    [SerializeField]
    private bool invertY;

    [SerializeField]
    private float defaultVerticalAngle = 20f;

    private Vector3 newPosition;

    private Quaternion newRotation;

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
        framing.x = -0.02f;
        framing.y = -0.69f;
    }

    public void adjustCameraForStanding()
    {
        framing.x = 0;
        framing.y = 0;
    }

    private void Start()
    {
        obstructionLayers = 1 << 2;
        obstructionLayers = ~obstructionLayers;
        framing = new Vector3(0, 0, 0);
        ignoredColliders.AddRange(GetComponentsInChildren<Collider>());
        planarDirection = playerTransform.forward;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        targetDistance = defaultDistance;
        targetVerticalAngle = defaultVerticalAngle;
        targetRotation =
            Quaternion.LookRotation(planarDirection) * Quaternion.Euler(targetVerticalAngle, 0, 0);
        targetPosition =
            playerTransform.position - (targetRotation * Vector3.forward) * targetDistance;
    }

    private void LateUpdate()
    {
        float zoom = -PlayerInputs.MouseScrollInput * zoomSpeed; // -0.5 lub 0.5
        float mouseX = PlayerInputs.MouseXInput; //wartosc od okolo -40 do 40 - 40 bardzo szybki ruch, 10 srednio szybki,
        float mouseY = -PlayerInputs.MouseYInput;
        if (invertX)
        {
            mouseX *= -1;
        }
        if (invertY)
        {
            mouseY *= -1;
        }

        Vector3 focusPosition = playerTransform.position + new Vector3(framing.x, framing.y, 0);

        planarDirection = Quaternion.Euler(0, mouseX, 0) * planarDirection; //rotate planar direction (mouseX degrees) around y axis
        targetDistance = Mathf.Clamp(targetDistance + zoom, minDistance, maxDistance); //zwiekszamy/zmniejszamy dystans o wartosc _zoom
        targetVerticalAngle = Mathf.Clamp(
            targetVerticalAngle + mouseY,
            minVerticalAngle,
            maxVerticalAngle
        );

        float smallestDistance = targetDistance;
        RaycastHit hit;
        bool didHit = Physics.Raycast(
            playerTransform.position,
            -camera.transform.forward,
            out hit,
            Vector3.Distance(camera.transform.position, playerTransform.position),
            obstructionLayers
        );

        if (didHit && hit.collider.gameObject != gameObject)
        {
            smallestDistance = hit.distance;
        }

        targetRotation =
            Quaternion.LookRotation(planarDirection) * Quaternion.Euler(targetVerticalAngle, 0, 0);
        targetPosition = focusPosition - (targetRotation * Vector3.forward) * smallestDistance;
        newRotation = Quaternion.Slerp(
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
            case EventType.QUEST_CONFIRMATION_DONE:
                enabled = true;
                break;
        }
    }
}
