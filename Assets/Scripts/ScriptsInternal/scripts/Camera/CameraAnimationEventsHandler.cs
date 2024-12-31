using UnityEngine;

public class CameraAnimationEventsHandler : MonoBehaviour
{
    private CharacterController characterController;

    private void Start()
    {
        characterController = FindAnyObjectByType<CharacterController>();
    }

    public void PushWall()
    {
        WallPushButton wallPush = (WallPushButton)
            characterController.playerState.objectToInteractWith;
        wallPush.PushWall();
    }
}
