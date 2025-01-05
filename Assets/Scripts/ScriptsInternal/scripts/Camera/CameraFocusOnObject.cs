using UnityEngine;

public class CameraFocusOnObject : Observer
{
    private Camera cam;
    private CameraController cameraController;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cameraController = GetComponent<CameraController>();
    }

    private void EnableCameraController()
    {
        cameraController.enabled = true;
        Time.timeScale = 1;
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.LEVER_OPENING:
                ObjectWithPositionDTO objectWithPosition = (ObjectWithPositionDTO)
                    eventDTO.eventData;
                cam.transform.position = objectWithPosition.position;
                cam.transform.rotation = objectWithPosition.rotation;
                break;
            case EventType.LEVER_OPENED:
                ObjectWithPositionDTO eventData = (ObjectWithPositionDTO)eventDTO.eventData;
                GameObject gate = eventData.gameObject;
                cam.transform.position = eventData.position;
                cam.transform.LookAt(gate.transform.position);
                break;
        }
    }
}
