[System.Serializable]
public class GridPosition
{
    public int xPosition { get; private set; }
    public int yPosition { get; private set; }

    public GridPosition(int xPosition, int yPosition)
    {
        this.xPosition = xPosition;
        this.yPosition = yPosition;
    }
}
