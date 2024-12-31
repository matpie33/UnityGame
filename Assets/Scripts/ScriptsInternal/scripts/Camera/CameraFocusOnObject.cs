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
            case EventType.FALLING_BALL_TRIGGERED:
                cameraController.enabled = false;
                GameObject gameObject = (GameObject)eventDTO.eventData;
                cam.transform.position = gameObject.transform.position - transform.forward * 3f;
                cam.transform.LookAt(gameObject.transform);
                Time.timeScale = 0.3f;
                Invoke(nameof(EnableCameraController), 1 * Time.timeScale);
                break;

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
