using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrapFieldsInitializer6x6 : TrapFieldsInitializer
{
    private int horizontalSize = 6;
    private int verticalSize = 6;

    public override GridSize GetGridSize()
    {
        return new(horizontalSize, verticalSize);
    }

    public override List<GridPositionAndDirection> GetTriggerFields()
    {
        List<GridPositionAndDirection> coords = new();

        AddTrapsInRow(0, Direction.FORWARD, coords, 0, 1, 3, 5);
        AddTrapsInRow(1, Direction.FORWARD, coords, 2, 3, 4, 5);
        AddTrapsInRow(1, Direction.LEFT, coords, 1);
        AddTrapsInRow(2, Direction.FORWARD, coords, 0, 1, 2, 3);
        AddTrapsInRow(2, Direction.RIGHT, coords, 4);
        AddTrapsInRow(3, Direction.LEFT, coords, 4);
        AddTrapsInRow(3, Direction.FORWARD, coords, 5);
        AddTrapsInRow(4, Direction.FORWARD, coords, 1, 2, 3, 4, 5);

        return coords;
    }

    private void AddTrapsInRow(
        int row,
        Direction direction,
        List<GridPositionAndDirection> coordsList,
        params int[] columns
    )
    {
        foreach (int column in columns)
        {
            coordsList.Add(new(new GridPosition(column, row), direction));
        }
    }
}
