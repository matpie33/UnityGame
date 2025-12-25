using System;
using System.Collections.Generic;
using UnityEngine;

public class TrapFieldsSynchronizedInitializer: MonoBehaviour
{

    [SerializeField]
    public int numberOfColumns;

    [SerializeField]
    public List<SpikeRow> spikeRows = new();

    private GameObject objectToCopy;

    private void Awake()
    {
        objectToCopy = transform.Find("spikeTrap").gameObject;
        Initialize();
    }


    public void Initialize()
    {

        SpikeTimedAnimationTrigger firstObjectTrigger = objectToCopy.GetComponentInChildren<SpikeTimedAnimationTrigger>();
        firstObjectTrigger.timeUp = spikeRows[0].timeUp;
        firstObjectTrigger.timeDown = spikeRows[0].timeDown;
        firstObjectTrigger.initialDelay = spikeRows[0].initialDelay;
        firstObjectTrigger.SetActive(spikeRows[0].inactiveSpikes == 0);

        for (int y = 0; y < spikeRows.Count; y++)
        {
            for (int x = 0; x < numberOfColumns; x++)
            {
                SpikeRow row = spikeRows[y];
                if ((x == 0 && y == 0))
                {
                    continue;
                }
                GameObject copiedObject = Instantiate(objectToCopy, transform);
                SpikeTimedAnimationTrigger spikeTimedTrigger = copiedObject.GetComponentInChildren<SpikeTimedAnimationTrigger>();
                spikeTimedTrigger.timeUp = spikeRows[y].timeUp;
                spikeTimedTrigger.timeDown = spikeRows[y].timeDown;
                spikeTimedTrigger.initialDelay = spikeRows[y].initialDelay;
                spikeTimedTrigger.SetActive(x >= row.inactiveSpikes);


                Vector3 tileSize = gameObject.transform.GetChild(0)
                    .Find("tiles")
                    .GetComponent<BoxCollider>()
                    .bounds.extents;
                copiedObject.transform.Translate(
                    Vector3.Scale(transform.forward * y, tileSize * 2),
                    Space.World
                );
                copiedObject.transform.Translate(
                    Vector3.Scale(transform.right * x, tileSize * 2),
                    Space.World
                );
                copiedObject.name = gameObject.name + x + y;
            }
        }

    }


}
