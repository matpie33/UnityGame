using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrapFieldsInitializer4x4 : TrapFieldsInitializer
{
    private int horizontalSize = 4;
    private int verticalSize = 6;

    public override GridSize GetGridSize()
    {
        return new(horizontalSize, verticalSize);
    }

    public override List<GridPositionAndDirection> GetTriggerFields()
    {
        List<GridPositionAndDirection> coords = new();

        for (int x = 0; x < 3; x++)
        {
            coords.Add(new(new(x, 0), Direction.FORWARD));
        }

        coords.Add(new(new GridPosition(0, 1), Direction.RIGHT));
        coords.Add(new(new GridPosition(1, 1), Direction.BACKWARD));
        coords.Add(new(new GridPosition(2, 1), Direction.BACKWARD));
        coords.Add(new(new GridPosition(3, 1), Direction.BACKWARD));

        for (int x = 0; x < 3; x++)
        {
            coords.Add(new(new(x, 2), Direction.FORWARD));
        }

        return coords;
    }
}
