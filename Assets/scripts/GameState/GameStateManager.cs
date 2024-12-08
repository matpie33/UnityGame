using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager
{
    private ISet<string> killedEnemiesIds = new HashSet<string>();
    public CheckpointData checkpointData { get; private set; }

    public void AddKilledEnemy(ObjectWithHealth enemy)
    {
        killedEnemiesIds.Add(enemy.GetUUid());
    }

    public ISet<string> GetKilledEnemiesUUids()
    {
        return killedEnemiesIds;
    }

    internal void SaveCheckpoint(GameObject checkpoint, int playerHealth)
    {
        checkpointData = new CheckpointData(playerHealth, checkpoint.transform.position);
    }
}
