using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUIUpdater : ScriptableObject
{
    public static void UpdateUI(Image healthBar, HealthState healthState)
    {
        healthBar.fillAmount = Mathf.MoveTowards(
            healthBar.fillAmount,
            (float)healthState.value / (float)healthState.maxHealth,
            Time.deltaTime * 1
        );
    }
}
