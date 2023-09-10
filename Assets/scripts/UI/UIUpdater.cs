using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [SerializeField]
    private GameObject statsPanel;

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

    public void SetRemainingStatsToAdd(int statsLeft, PlayerUI playerUI)
    {
        playerUI.statsLeftText.text = "" + statsLeft;
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

    public void InitializeStatsPanel(
        PlayerState playerState,
        PlayerUI playerUI,
        StatsAddingDTO statsAddingDTO
    )
    {
        InitializeStatIncreasingButtons(playerUI, statsAddingDTO);
        SetVisibilityOfStatsPanel(false, playerUI.statsPanel);
        UpdateStatsUI(playerState, playerUI);
        ToggleVisibilityOfStatsModification(false, playerUI);
    }

    private void InitializeStatIncreasingButtons(PlayerUI playerUI, StatsAddingDTO statsAddingDTO)
    {
        Button addAgilityButton = playerUI.addAgilityButton;
        addAgilityButton.onClick.AddListener(() =>
        {
            if (statsAddingDTO.statsLeft > 0)
            {
                statsAddingDTO.IncreaseAgility();
                IncreaseStat(addAgilityButton, statsAddingDTO, playerUI);
            }
        });
        Button addDefenceButton = playerUI.addDefenceButton;
        addDefenceButton.onClick.AddListener(() =>
        {
            if (statsAddingDTO.statsLeft > 0)
            {
                statsAddingDTO.IncreaseDefence();
                IncreaseStat(addDefenceButton, statsAddingDTO, playerUI);
            }
        });
        Button healthButton = playerUI.addHealthButton;
        healthButton.onClick.AddListener(() =>
        {
            if (statsAddingDTO.statsLeft > 0)
            {
                statsAddingDTO.IncreaseHealth();
                IncreaseStat(healthButton, statsAddingDTO, playerUI);
            }
        });
        Button addStrengthButton = playerUI.addStrengthButton;
        addStrengthButton.onClick.AddListener(() =>
        {
            if (statsAddingDTO.statsLeft > 0)
            {
                statsAddingDTO.IncreaseStrength();
                IncreaseStat(addStrengthButton, statsAddingDTO, playerUI);
            }
        });
    }

    private void IncreaseStat(Button button, StatsAddingDTO statsAddingDTO, PlayerUI playerUI)
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

    public void SetVisibilityOfStatsPanel(bool visible, GameObject statsPanel)
    {
        statsPanel.SetActive(visible);
    }

    internal void UpdateStatsUI(PlayerState playerState, PlayerUI playerUI)
    {
        playerUI.agilityStat.text = "" + playerState.agility;
        playerUI.healthStat.text = "" + playerState.health;
        playerUI.defenceStat.text = "" + playerState.defence;
        playerUI.strengthStat.text = "" + playerState.strength;
    }

    private void Update()
    {
        if (ActionKeys.IsKeyPressedWithControl(ActionKeys.OPEN_STATS_PANEL))
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
    }

    public void ToggleVisibilityOfStatsModification(bool visible, PlayerUI playerUI)
    {
        playerUI.addStrengthButton.gameObject.SetActive(visible);
        playerUI.addHealthButton.gameObject.SetActive(visible);
        playerUI.addDefenceButton.gameObject.SetActive(visible);
        playerUI.addAgilityButton.gameObject.SetActive(visible);
        playerUI.statsBottomPanel.SetActive(visible);
    }
}
