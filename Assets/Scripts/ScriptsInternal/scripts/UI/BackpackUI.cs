using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackpackUI : MonoBehaviour
{
    private const float LERP_CONSTANT = 0.13f;
    private List<Pickable> objectsInBackpack;

    private int currentObjectIndex;

    private Pickable currentlySelectedObject;

    [SerializeField]
    private GameObject previousObjectPlaceholder;

    [SerializeField]
    private GameObject currentObjectPlaceholder;

    [SerializeField]
    private GameObject nextObjectPlaceholder;

    [SerializeField]
    private TextMeshProUGUI descriptionTextField;

    [SerializeField]
    private GameObject backpackPanel;

    private GameObject previousObject;
    private GameObject currentObject;
    private GameObject nextObject;

    [SerializeField]
    private GameObject blurringBackground;

    private CharacterController characterController;

    private Camera cameraObject;

    private EventQueue eventQueue;

    private bool moveRight;
    private bool moveLeft;
    private CameraController cameraController;
    private GameObject player;

    [SerializeField]
    private float cameraSlerpValue;

    private bool rotateCamera;

    private Quaternion targetCameraRotation;
    private GameManager gameManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cameraController = FindAnyObjectByType<CameraController>();
        eventQueue = FindAnyObjectByType<EventQueue>();
        cameraObject = FindAnyObjectByType<Camera>();
        blurringBackground.SetActive(false);
        backpackPanel.SetActive(false);
        characterController = FindAnyObjectByType<CharacterController>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Toggle()
    {
        bool isThisOpeningEvent = !backpackPanel.activeSelf;
        eventQueue.SubmitEvent(
            new EventDTO(EventType.BACKPACK_OPEN_CLOSE_EVENT, isThisOpeningEvent)
        );
        if (backpackPanel.activeSelf)
        {
            Hide();
            Time.timeScale = 1;
        }
        else
        {
            Show();
            Time.timeScale = 0;
            objectsInBackpack = characterController.playerBackpack.GetObjects();
            if (objectsInBackpack.Count > 0)
            {
                currentObjectIndex = 0;
                currentlySelectedObject = objectsInBackpack[0];
                DisplayObjects();
            }
        }
    }

    internal void Hide()
    {
        player.SetActive(true);
        cameraController.enabled = true;
        Destroy(currentObject);
        Destroy(previousObject);
        Destroy(nextObject);
        blurringBackground.SetActive(false);
        backpackPanel.SetActive(false);
        rotateCamera = false;
    }

    public void Show()
    {
        player.SetActive(false);
        cameraController.enabled = false;
        blurringBackground.SetActive(true);
        backpackPanel.SetActive(true);
        rotateCamera = true;
        targetCameraRotation = Quaternion.Euler(
            0,
            cameraObject.transform.rotation.eulerAngles.y,
            0
        );
    }

    private void DisplayObjects()
    {
        PickableDefinition definition = currentlySelectedObject.definition;
        currentObject = InstantiateFromDefintion(
            currentlySelectedObject.gameObject,
            currentObjectPlaceholder
        );
        currentObject.GetComponent<RotatingObject>().enabled = true;

        string description = definition.description;

        descriptionTextField.text = description;
        AddNextObjectOptionally();
        AddPreviousObjectOptionally();
    }

    private GameObject InstantiateFromDefintion(GameObject model, GameObject parent)
    {
        GameObject clone = Instantiate(model);
        clone.transform.localScale = Vector3.one * 1f;
        clone.SetActive(true);
        RotatingObject rotatingObject = clone.AddComponent<RotatingObject>();
        rotatingObject.rotationSpeed = 1;
        rotatingObject.ignoreTimescale = true;
        rotatingObject.rotationDirection = new Vector3(0, 1, 0);
        rotatingObject.enabled = false;
        clone.transform.parent = parent.transform;
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.Euler(0, 0, 0);
        return clone;
    }

    private void AddNextObjectOptionally()
    {
        if (IsNotLastItem())
        {
            Pickable pickable = objectsInBackpack[currentObjectIndex + 1];
            nextObject = InstantiateFromDefintion(pickable.gameObject, nextObjectPlaceholder);
        }
    }

    private void AddPreviousObjectOptionally()
    {
        if (IsNotFirstItem())
        {
            Pickable pickable = objectsInBackpack[currentObjectIndex - 1];
            previousObject = InstantiateFromDefintion(
                pickable.gameObject,
                previousObjectPlaceholder
            );
        }
    }

    private bool IsNotLastItem()
    {
        return currentObjectIndex + 1 < objectsInBackpack.Count;
    }

    private bool IsNotFirstItem()
    {
        return currentObjectIndex - 1 >= 0;
    }

    private void Update()
    {
        if (moveLeft || moveRight)
        {
            Vector3 previousObjectPosition = previousObjectPlaceholder.transform.position;
            Vector3 currentObjectPosition = currentObjectPlaceholder.transform.position;
            Vector3 nextObjectPosition = nextObjectPlaceholder.transform.position;

            Vector3 currentObjectDestination = moveRight
                ? previousObjectPosition
                : nextObjectPosition;
            currentObject.transform.position = Vector3.Lerp(
                currentObject.transform.position,
                currentObjectDestination,
                LERP_CONSTANT
            );
            GameObject nextOrPrevious = (moveRight ? nextObject : previousObject);
            nextOrPrevious.transform.position = Vector3.Lerp(
                nextOrPrevious.transform.position,
                currentObjectPosition,
                LERP_CONSTANT
            );
            if (
                Vector3.Distance(currentObject.transform.position, currentObjectDestination) < 0.01f
            )
            {
                if (moveRight)
                {
                    previousObject = currentObject;
                    nextObject = null;
                    AddNextObjectOptionally();
                }
                else
                {
                    nextObject = currentObject;
                    previousObject = null;
                    AddPreviousObjectOptionally();
                }

                currentObject = nextOrPrevious;
                currentObject.GetComponent<RotatingObject>().enabled = true;
                moveLeft = false;
                moveRight = false;
            }
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.OPEN_BACKPACK) && gameManager.interruptableAnimationsHandler == null)
        {
            Toggle();
        }
        if (!backpackPanel.activeSelf)
        {
            return;
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.LEFT_KEY))
        {
            MoveLeft();
        }

        if (ActionKeys.IsKeyPressed(ActionKeys.RIGHT_KEY))
        {
            MoveRight();
        }
        if (rotateCamera)
        {
            cameraObject.transform.rotation = Quaternion.Lerp(
                cameraObject.transform.rotation,
                targetCameraRotation,
                cameraSlerpValue
            );
        }
    }

    private void MoveLeft()
    {
        if (IsNotFirstItem())
        {
            currentObjectIndex--;
            string description = objectsInBackpack[currentObjectIndex].definition.description;
            descriptionTextField.text = description;
            Destroy(nextObject);
            RotatingObject rotationScript = currentObject.GetComponent<RotatingObject>();
            rotationScript.enabled = false;
            moveLeft = true;
        }
    }

    private void MoveRight()
    {
        if (IsNotLastItem())
        {
            currentObjectIndex++;
            string description = objectsInBackpack[currentObjectIndex].definition.description;
            descriptionTextField.text = description;
            Destroy(previousObject);
            RotatingObject rotatingScript = currentObject.GetComponent<RotatingObject>();
            rotatingScript.enabled = false;
            moveRight = true;
        }
    }
}
