using System.Collections.Generic;
using UnityEngine;

public abstract class TrapFieldsInitializer : MonoBehaviour
{
    public abstract List<GridPositionAndDirection> GetTriggerFields();

    public abstract GridSize GetGridSize();
}
