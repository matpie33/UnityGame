using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    public void UpdateMedipackAmount(TextMeshProUGUI textComponent, int newValue)
    {
        textComponent.text = "" + newValue;
    }

    public void UpdateHealthBar(
        HealthState healthState,
        TextMeshProUGUI textComponent,
        Image healthBar
    )
    {
        healthBar.fillAmount = Mathf.MoveTowards(
            healthBar.fillAmount,
            (float)healthState.value / (float)healthState.maxHealth,
            Time.deltaTime * 1
        );
        textComponent.text = healthState.value + "/" + healthState.maxHealth;
    }

    public void UpdateExperience(
        LevelData levelData,
        TextMeshProUGUI experienceText,
        Image experienceBar,
        TextMeshProUGUI levelText
    )
    {
        experienceBar.fillAmount = Mathf.MoveTowards(
            experienceBar.fillAmount,
            (float)levelData.experience / (float)levelData.experienceNeededForNextLevel,
            Time.deltaTime * 1
        );
        experienceText.text = levelData.experience + "/" + levelData.experienceNeededForNextLevel;
        levelText.text = "" + levelData.level;
    }
}
