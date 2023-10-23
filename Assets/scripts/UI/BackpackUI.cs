using System;
using System.Collections;
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

    private void Start()
    {
        eventQueue = FindObjectOfType<EventQueue>();
        cameraObject = FindObjectOfType<Camera>();
        blurringBackground.SetActive(false);
        backpackPanel.SetActive(false);
        characterController = FindObjectOfType<CharacterController>();
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
        Destroy(currentObject);
        Destroy(previousObject);
        Destroy(nextObject);
        blurringBackground.SetActive(false);
        backpackPanel.SetActive(false);
    }

    public void Show()
    {
        cameraObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        blurringBackground.SetActive(true);
        backpackPanel.SetActive(true);
    }

    private void DisplayObjects()
    {
        PickableDefinition definition = currentlySelectedObject.definition;
        GameObject model = definition.model;
        currentObject = Instantiate(model);
        currentObject.AddComponent<RotatingObject>();
        currentObject.transform.parent = currentObjectPlaceholder.transform;
        currentObject.transform.localPosition = Vector3.zero;
        string description = definition.description;

        descriptionTextField.text = description;
        AddNextObjectOptionally();
        AddPreviousObjectOptionally();
    }

    private void AddNextObjectOptionally()
    {
        if (IsNotLastItem())
        {
            PickableDefinition definition = objectsInBackpack[currentObjectIndex + 1].definition;
            nextObject = Instantiate(definition.model);
            nextObject.transform.parent = nextObjectPlaceholder.transform;
            nextObject.transform.localPosition = Vector3.zero;
        }
    }

    private void AddPreviousObjectOptionally()
    {
        if (IsNotFirstItem())
        {
            PickableDefinition previousDefinition = objectsInBackpack[
                currentObjectIndex - 1
            ].definition;
            previousObject = Instantiate(previousDefinition.model);
            previousObject.transform.parent = previousObjectPlaceholder.transform;
            previousObject.transform.localPosition = Vector3.zero;
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
                string description = objectsInBackpack[currentObjectIndex].definition.description;
                descriptionTextField.text = description;
                currentObject = nextOrPrevious;
                currentObject.AddComponent<RotatingObject>();
                moveLeft = false;
                moveRight = false;
            }
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.OPEN_BACKPACK))
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
    }

    private void MoveLeft()
    {
        if (IsNotFirstItem())
        {
            currentObjectIndex--;
            Destroy(nextObject);
            RotatingObject rotationScript = currentObject.GetComponent<RotatingObject>();
            Destroy(rotationScript);
            moveLeft = true;
        }
    }

    private void MoveRight()
    {
        if (IsNotLastItem())
        {
            currentObjectIndex++;
            Destroy(previousObject);
            RotatingObject rotatingScript = currentObject.GetComponent<RotatingObject>();
            Destroy(rotatingScript);
            moveRight = true;
        }
    }
}
