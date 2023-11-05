using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreUpdate;
using UnityEngine.Playables;

public class ButtonClickHandler : MonoBehaviour
{
    private CharacterController characterController;
    private UIUpdater uiUpdater;
    private StatsToValuesConverter statsToValuesConverter;

    private void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
        uiUpdater = GetComponent<UIUpdater>();
        statsToValuesConverter = new StatsToValuesConverter();
    }

    public void ResetStats()
    {
        uiUpdater.ClearStatsInUI(characterController.playerState);
    }

    public void AcceptStats()
    {
        characterController.playerState.increaseStats(uiUpdater.statsAddingDTO);
        characterController.healthState.IncreaseMaxHealth(
            statsToValuesConverter.ConvertHealthStatToHPIncrease(
                characterController.playerState.health
            )
        );
        characterController.animationsManager.setAttackSpeed(
            statsToValuesConverter.ConvertAgilityToAttackSpeed(
                characterController.playerState.agility
            )
        );
        uiUpdater.SetVisibilityOfStatsModification(false);
        uiUpdater.ResetStats();
        uiUpdater.HideAddStatsIcon();
    }
}
