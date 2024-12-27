[System.Serializable]
public class GridPositionAndDirection
{
    public GridPosition gridPosition { get; private set; }

    public Direction direction { get; private set; }

    public GridPositionAndDirection(GridPosition coordinate, Direction direction)
    {
        this.gridPosition = coordinate;
        this.direction = direction;
    }
}
