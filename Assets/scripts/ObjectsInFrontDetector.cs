using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectsInFrontDetector : MonoBehaviour
{
    public float minDistance = 3;

    [SerializeField]
    private Transform feetLevel;

    [SerializeField]
    private GameObject pickupHint;

    private PickupObjectsController pickupObjectsController;

    private void Start()
    {
        pickupObjectsController = GetComponent<PickupObjectsController>();
    }

    private void Update()
    {
        Vector3 position = feetLevel.position;
        RaycastHit hit;
        if (Physics.Raycast(position, transform.forward, out hit, Mathf.Infinity))
        {
            float distance = hit.distance;
            if (hit.collider.tag.Equals("Pickable") && distance < minDistance)
            {
                pickupHint.SetActive(true);
                pickupObjectsController.objectInFront = hit.collider.gameObject;
            }
            else
            {
                pickupHint.SetActive(false);
                pickupObjectsController.objectInFront = null;
            }
        }
        else
        {
            pickupHint.SetActive(false);
            pickupObjectsController.objectInFront = null;
        }
    }
}
