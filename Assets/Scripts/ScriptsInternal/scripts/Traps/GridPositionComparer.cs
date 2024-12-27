using System.Collections.Generic;

public class GridPositionComparer : IEqualityComparer<GridPosition>
{
    public bool Equals(GridPosition obj1, GridPosition obj2)
    {
        if (ReferenceEquals(obj1, obj2))
            return true;

        if (obj1 is null || obj2 is null)
            return false;

        return obj1.xPosition == obj2.xPosition && obj1.yPosition == obj2.yPosition;
    }

    public int GetHashCode(GridPosition coordinate) => coordinate.xPosition ^ coordinate.yPosition;
}
