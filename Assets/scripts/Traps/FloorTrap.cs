using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorTrap : MonoBehaviour
{
    [SerializeField]
    private GameObject spikes;

    [SerializeField]
    private Direction directionForSpike;

    private GameObject spikesPrefab;
    private bool hideOldSpike;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals(Tags.PLAYER))
        {
            hideOldSpike = false;
            if (spikesPrefab == null)
            {
                spikesPrefab = Instantiate(spikes);
            }
            else
            {
                spikesPrefab.GetComponent<Animator>().Play("Base Layer.SpikesUp");
            }
            Vector3 offset;
            Vector3 boxExtents = new Vector3(2, 2, 2);
            float offsetY = spikesPrefab.GetComponent<BoxCollider>().size.y;
            switch (directionForSpike)
            {
                case Direction.FORWARD:
                    offset = transform.forward * boxExtents.z;
                    break;
                case Direction.BACKWARD:
                    offset = -transform.forward * boxExtents.z;
                    ;
                    break;
                case Direction.LEFT:
                    offset = -transform.right * boxExtents.x;
                    break;
                case Direction.RIGHT:
                    offset = transform.right * boxExtents.x;
                    break;
                default:
                    throw new Exception("No enum found");
            }
            spikesPrefab.transform.position =
                gameObject.transform.position - offsetY * Vector3.up + offset;
            spikesPrefab.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag.Equals(Tags.PLAYER))
        {
            spikesPrefab.GetComponent<Animator>().Play("Base Layer.SpikesDown");
            hideOldSpike = true;
            Invoke(nameof(DeactivatePrefab), 2);
        }
    }

    private void DeactivatePrefab()
    {
        if (hideOldSpike)
        {
            spikesPrefab.SetActive(false);
        }
    }
}
