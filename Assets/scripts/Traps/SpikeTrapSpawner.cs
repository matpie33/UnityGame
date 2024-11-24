using System;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapSpawner : MonoBehaviour
{
    private TrapFieldsInitializer trapFieldsInitializer;

    private Dictionary<GridPosition, GameObject> triggersPositions;

    public void Initialize()
    {
        trapFieldsInitializer = GetComponent<TrapFieldsInitializer>();
        triggersPositions = new(new GridPositionComparer());
        GridSize gridSize = trapFieldsInitializer.GetGridSize();
        int gridVerticalSize = gridSize.vertical;
        int gridHorizontalSize = gridSize.horizontal;

        List<GridPositionAndDirection> triggerFields = trapFieldsInitializer.GetTriggerFields();

        triggersPositions.Add(new GridPosition(0, 0), gameObject);
        for (int y = 0; y < gridVerticalSize; y++)
        {
            for (int x = 0; x < gridHorizontalSize; x++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                GameObject copiedObject = Instantiate(gameObject, gameObject.transform.parent);

                Vector3 tileSize = gameObject.transform
                    .Find("tiles")
                    .GetComponent<BoxCollider>()
                    .bounds.extents;
                copiedObject.transform.Translate(
                    Vector3.Scale(gameObject.transform.forward * y, tileSize * 2),
                    Space.World
                );
                copiedObject.transform.Translate(
                    Vector3.Scale(gameObject.transform.right * x, tileSize * 2),
                    Space.World
                );
                copiedObject.name = gameObject.name + x + y;
                Destroy(copiedObject.GetComponent<SpikeTrapSpawner>());
                triggersPositions.Add(new(x, y), copiedObject);
            }
        }

        foreach (GridPositionAndDirection gridPositionAndDirection in triggerFields)
        {
            GridPosition triggeringFieldPosition = gridPositionAndDirection.gridPosition;
            GameObject triggeringField = triggersPositions[gridPositionAndDirection.gridPosition];
            SpikeTrigger trigger =
                triggeringField.transform.Find("tile").gameObject.AddComponent(typeof(SpikeTrigger))
                as SpikeTrigger;

            GridPosition triggeredFieldPosition;
            switch (gridPositionAndDirection.direction)
            {
                case Direction.FORWARD:
                    triggeredFieldPosition = new(
                        triggeringFieldPosition.xPosition,
                        triggeringFieldPosition.yPosition + 1
                    );
                    break;
                case Direction.BACKWARD:
                    triggeredFieldPosition = new(
                        triggeringFieldPosition.xPosition,
                        triggeringFieldPosition.yPosition - 1
                    );
                    break;
                case Direction.RIGHT:
                    triggeredFieldPosition = new(
                        triggeringFieldPosition.xPosition + 1,
                        triggeringFieldPosition.yPosition
                    );
                    break;
                case Direction.LEFT:
                    triggeredFieldPosition = new(
                        triggeringFieldPosition.xPosition - 1,
                        triggeringFieldPosition.yPosition
                    );
                    break;
                default:
                    throw new Exception("Unknown direction: " + gridPositionAndDirection.direction);
            }
            GameObject spikeToTrigger = triggersPositions[triggeredFieldPosition];
            trigger.spikeToTrigger = spikeToTrigger;
        }
    }
}
