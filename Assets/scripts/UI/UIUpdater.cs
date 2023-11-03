using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.PlayerLoop.PreUpdate;

public class UIUpdater : MonoBehaviour
{
    [SerializeField]
    private GameObject statsPanel;

    private PlayerUI playerUI;

    private CharacterController characterController;

    public StatsAddingDTO statsAddingDTO { get; private set; }

    private void Awake()
    {
        playerUI = GetComponent<PlayerUI>();
        playerUI.healthBar.fillAmount = 1;
        statsAddingDTO = new StatsAddingDTO();
        characterController = FindObjectOfType<CharacterController>();
    }

    public void UpdateMedipackAmount(int newValue)
    {
        TextMeshProUGUI textComponent = playerUI.medipackAmountText;
        textComponent.text = "" + newValue;
    }

    public void ResetStats()
    {
        statsAddingDTO.Reset();
        SetRemainingStatsToAdd(statsAddingDTO.statsLeft);
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

    private void SetRemainingStatsToAdd(int statsLeft)
    {
        playerUI.statsLeftText.text = "" + statsLeft;
    }

    public void UpdateExperience(LevelData levelData)
    {
        TextMeshProUGUI experienceText = playerUI.experienceText;
        Image experienceBar = playerUI.experienceBar;
        TextMeshProUGUI levelText = playerUI.levelText;
        experienceBar.fillAmount = Mathf.MoveTowards(
            experienceBar.fillAmount,
            (float)levelData.experience / (float)levelData.experienceNeededForNextLevel,
            Time.deltaTime * 1
        );
        experienceText.text = levelData.experience + "/" + levelData.experienceNeededForNextLevel;
        levelText.text = "" + levelData.level;
    }

    public void InitializeStatsPanel(PlayerState playerState)
    {
        InitializeStatIncreasingButtons();
        SetVisibilityOfStatsPanel(false);
        ClearStatsInUI(playerState);
        SetVisibilityOfStatsModification(false);
    }

    private void InitializeStatIncreasingButtons()
    {
        Button addAgilityButton = playerUI.addAgilityButton;
        addAgilityButton.onClick.AddListener(() =>
        {
            if (statsAddingDTO.statsLeft > 0)
            {
                statsAddingDTO.IncreaseAgility();
                IncreaseStat(addAgilityButton, statsAddingDTO);
            }
        });
        Button addDefenceButton = playerUI.addDefenceButton;
        addDefenceButton.onClick.AddListener(() =>
        {
            if (statsAddingDTO.statsLeft > 0)
            {
                statsAddingDTO.IncreaseDefence();
                IncreaseStat(addDefenceButton, statsAddingDTO);
            }
        });
        Button healthButton = playerUI.addHealthButton;
        healthButton.onClick.AddListener(() =>
        {
            if (statsAddingDTO.statsLeft > 0)
            {
                statsAddingDTO.IncreaseHealth();
                IncreaseStat(healthButton, statsAddingDTO);
            }
        });
        Button addStrengthButton = playerUI.addStrengthButton;
        addStrengthButton.onClick.AddListener(() =>
        {
            if (statsAddingDTO.statsLeft > 0)
            {
                statsAddingDTO.IncreaseStrength();
                IncreaseStat(addStrengthButton, statsAddingDTO);
            }
        });
    }

    private void IncreaseStat(Button button, StatsAddingDTO statsAddingDTO)
    {
        Transform parent = button.transform.parent;
        GameObject statValueText = parent.Find("Value").gameObject;
        TextMeshProUGUI statValueTMPRO = statValueText.GetComponent<TextMeshProUGUI>();
        int oldValue = int.Parse(statValueTMPRO.text);
        oldValue++;
        statValueTMPRO.text = "" + oldValue;
        statsAddingDTO.statsLeft--;
        playerUI.statsLeftText.text = "" + statsAddingDTO.statsLeft;
    }

    private void SetVisibilityOfStatsPanel(bool visible)
    {
        playerUI.statsPanel.SetActive(visible);
    }

    internal void ClearStatsInUI(PlayerState playerState)
    {
        statsAddingDTO.Reset();
        SetRemainingStatsToAdd(statsAddingDTO.statsLeft);
        playerUI.agilityStat.text = "" + playerState.agility;
        playerUI.healthStat.text = "" + playerState.health;
        playerUI.defenceStat.text = "" + playerState.defence;
        playerUI.strengthStat.text = "" + playerState.strength;
    }

    private void Update()
    {
        if (ActionKeys.IsKeyPressed(ActionKeys.OPEN_STATS_PANEL))
        {
            statsPanel.SetActive(!statsPanel.activeSelf);
            bool isActive = statsPanel.activeSelf;
            if (isActive)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.OPEN_BACKPACK))
        {
            statsPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        UpdatePlayerHealth(characterController.healthState);
        UpdateExperience(characterController.levelData);
    }

    public void SetVisibilityOfStatsModification(bool visible)
    {
        playerUI.addStrengthButton.gameObject.SetActive(visible);
        playerUI.addHealthButton.gameObject.SetActive(visible);
        playerUI.addDefenceButton.gameObject.SetActive(visible);
        playerUI.addAgilityButton.gameObject.SetActive(visible);
        playerUI.statsBottomPanel.SetActive(visible);
    }

    internal void UpdatePlayerHealth(HealthState healthState)
    {
        UpdateHealthBar(healthState, playerUI.healthText, playerUI.healthBar);
    }
}
