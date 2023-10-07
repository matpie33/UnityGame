using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Normal attack")]
public class AttackAnimation : ScriptableObject
{
    public AnimatorOverrideController animatorOverride;
    public KeyCode key;
}
