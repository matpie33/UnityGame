using System.Collections.Generic;
using System.Linq;

public class GameStateManager
{
    private Dictionary<string, bool> killedEnemiesWithCheckpointFlag =
        new Dictionary<string, bool>();
    public ISet<string> openedGates { get; private set; }
    public ISet<string> openedLevers { get; private set; }

    public CheckpointData checkpointData { get; private set; }

    public GameStateManager()
    {
        openedGates = new HashSet<string>();
        openedLevers = new HashSet<string>();
    }

    public void AddKilledEnemy(ObjectWithHealth enemy)
    {
        if (!killedEnemiesWithCheckpointFlag.ContainsKey(enemy.GetUUid()))
        {
            killedEnemiesWithCheckpointFlag.Add(enemy.GetUUid(), false);
        }
    }

    public void AddOpenedLever(Lever lever)
    {
        openedGates.Add(lever.gateToOpen.GetUUid());
        openedLevers.Add(lever.GetUUid());
    }

    public void ClearNotSavedKilledEnemies()
    {
        foreach (string s in killedEnemiesWithCheckpointFlag.Keys.ToList())
        {
            if (!killedEnemiesWithCheckpointFlag[s])
            {
                killedEnemiesWithCheckpointFlag.Remove(s);
            }
        }
    }

    public ISet<string> GetKilledEnemiesUUids()
    {
        return killedEnemiesWithCheckpointFlag.Select(p => p.Key).ToHashSet();
    }

    internal void SaveCheckpoint(Checkpoint checkpoint, int playerHealth)
    {
        foreach (string s in killedEnemiesWithCheckpointFlag.Keys.ToList())
        {
            killedEnemiesWithCheckpointFlag[s] = true;
        }
        checkpointData = new CheckpointData(playerHealth, checkpoint.transform.position);
    }
}
