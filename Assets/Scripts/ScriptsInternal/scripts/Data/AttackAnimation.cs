using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Normal attack")]
public class AttackAnimation : ScriptableObject
{
    public AnimatorOverrideController animatorOverride;
    public KeyCode key;
    public float animationPercentWhenNextAttackCanBePlayed;
}
