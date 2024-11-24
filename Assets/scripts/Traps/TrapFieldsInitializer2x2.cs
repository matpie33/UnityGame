using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrapFieldsInitializer2x2 : TrapFieldsInitializer
{
    private int horizontalSize = 2;
    private int verticalSize = 2;

    public override GridSize GetGridSize()
    {
        return new(horizontalSize, verticalSize);
    }

    public override List<GridPositionAndDirection> GetTriggerFields()
    {
        List<GridPositionAndDirection> coords =
            new()
            {
                new(new(0, 0), Direction.FORWARD),
                new(new(0, 1), Direction.RIGHT),
                new(new(1, 0), Direction.FORWARD),
                new(new(1, 1), Direction.LEFT)
            };

        return coords;
    }
}
