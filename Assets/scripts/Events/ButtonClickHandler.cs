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
        uiUpdater.ClearStatsInUI(characterController.GetStats());
    }

    public void AcceptStats()
    {
        characterController.GetStats().Increase(uiUpdater.statsAddingDTO);
        characterController.IncreaseMaxHealth(
            statsToValuesConverter.ConvertHealthStatToHPIncrease(
                characterController.GetStats().health
            )
        );
        uiUpdater.UpdatePlayerHealth(
            characterController.GetComponent<ObjectWithHealth>().healthState
        );
        characterController.animationsManager.setAttackSpeed(
            statsToValuesConverter.ConvertAgilityToAttackSpeed(
                characterController.GetStats().agility
            )
        );
        uiUpdater.SetVisibilityOfStatsModification(false);
        uiUpdater.ResetStats();
        uiUpdater.HideAddStatsIcon();
    }
}
