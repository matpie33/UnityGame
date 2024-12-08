using UnityEngine;

public class CheckpointData
{
    public int playerHealth { get; private set; }
    public Vector3 position { get; private set; }

    public CheckpointData(int playerHealth, Vector3 position)
    {
        this.playerHealth = playerHealth;
        this.position = position;
    }
}
