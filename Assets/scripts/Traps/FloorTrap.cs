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

    private BoxCollider boxCollider;

    private bool hideOldSpike;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals(Tags.PLAYER))
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
            switch (directionForSpike)
            {
                case Direction.FORWARD:
                    offset = transform.forward;
                    break;
                case Direction.BACKWARD:
                    offset = -transform.forward;
                    break;
                case Direction.LEFT:
                    offset = -transform.right;
                    break;
                case Direction.RIGHT:
                    offset = transform.right;
                    break;
                default:
                    throw new Exception("No enum found");
            }
            float halfSize = boxCollider.bounds.size.magnitude / 2;
            spikesPrefab.transform.position =
                gameObject.transform.position - Vector3.up * halfSize * 1.5f + offset * halfSize;
            spikesPrefab.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag.Equals(Tags.PLAYER))
        {
            spikesPrefab.GetComponent<Animator>().Play("Base Layer.SpikesDown");
            hideOldSpike = true;
            Invoke("DeactivatePrefab", 2);
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
