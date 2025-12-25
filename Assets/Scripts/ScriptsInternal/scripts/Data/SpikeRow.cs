
[System.Serializable]
public class SpikeRow
{
    public float timeUp;

    public float timeDown;

    public int initialDelay;

    public int inactiveSpikes;

    public SpikeRow(float timeUp, float timeDown, int initialDelay, int inactiveSpikes)
    {
        this.timeUp = timeUp;
        this.timeDown = timeDown;
        this.initialDelay = initialDelay;
        this.inactiveSpikes = inactiveSpikes;
    }
}
