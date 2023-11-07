using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/quest")]
public class Quest : ScriptableObject
{
    [field: SerializeField]
    public string summary { get; private set; }

    [field: SerializeField]
    public List<string> questParts { get; private set; }
}
