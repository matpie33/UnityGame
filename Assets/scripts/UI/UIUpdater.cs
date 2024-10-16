﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : Observer
{
    [SerializeField]
    private GameObject statsPanel;

    private PlayerUI playerUI;

    private CharacterController characterController;

    public StatsAddingDTO statsAddingDTO { get; private set; }

    [SerializeField]
    private GameObject addStatsIcon;

    [SerializeField]
    private GameObject questPanel;

    [SerializeField]
    private Canvas healthBarPrefab;

    private List<ObjectWithHealth> objectsWithHealth = new List<ObjectWithHealth>();

    private void Awake()
    {
        playerUI = GetComponent<PlayerUI>();
        playerUI.healthBar.fillAmount = 1;
        statsAddingDTO = new StatsAddingDTO();
        characterController = FindAnyObjectByType<CharacterController>();
        addStatsIcon.SetActive(false);
        objectsWithHealth = FindObjectsByType<ObjectWithHealth>(FindObjectsSortMode.None).ToList();
        foreach (ObjectWithHealth objectWithHealth in objectsWithHealth)
        {
            if (!objectWithHealth.skipHealthBar)
            {
                CreateHealthBarForObject(objectWithHealth);
            }
        }
    }

    private void CreateHealthBarForObject(ObjectWithHealth objectWithHealth)
    {
        Canvas healthBar = Instantiate(healthBarPrefab);
        float halfHeight = objectWithHealth.GetComponent<Collider>().bounds.extents.y;
        Transform healthBarTransform = healthBar.transform;
        TextMeshProUGUI hpTextField = findHpTextInObject(healthBar.gameObject);
        HealthState healthState = objectWithHealth.healthState;
        hpTextField.text = healthState.value + "/" + healthState.maxHealth;
        Vector3 position = healthBarTransform.position;
        position.y = 2 * halfHeight;
        healthBarTransform.position = position;
        Image image = findHealthBarForegroundInObject(healthBar.gameObject);
        image.fillAmount = 1;

        healthBar.transform.SetParent(objectWithHealth.transform, false);
    }

    private Image findHealthBarForegroundInObject(GameObject gameObject)
    {
        if (gameObject.tag.Equals(Tags.PLAYER))
        {
            return playerUI.healthBar;
        }

        foreach (Image image in gameObject.GetComponentsInChildren<Image>())
        {
            if (image.gameObject.name.Equals("Foreground"))
            {
                return image;
            }
        }
        throw new Exception("Image with name foreground is not found for: " + gameObject);
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

    public void InitializeStatsPanel(Stats stats)
    {
        InitializeStatIncreasingButtons();
        SetVisibilityOfStatsPanel(false);
        ClearStatsInUI(stats);
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

    internal void ClearStatsInUI(Stats stats)
    {
        statsAddingDTO.Reset();
        SetRemainingStatsToAdd(statsAddingDTO.statsLeft);
        playerUI.agilityStat.text = "" + stats.agility;
        playerUI.healthStat.text = "" + stats.health;
        playerUI.defenceStat.text = "" + stats.defence;
        playerUI.strengthStat.text = "" + stats.strength;
    }

    private void Update()
    {
        if (ActionKeys.IsKeyPressed(ActionKeys.OPEN_STATS_PANEL))
        {
            OpenStatsPanel();
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.OPEN_BACKPACK))
        {
            statsPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (ActionKeys.IsKeyPressed(ActionKeys.MOUSE_ENABLE))
        {
            CursorLockMode lockState = Cursor.lockState;
            Cursor.lockState = lockState.Equals(CursorLockMode.Locked)
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        }
        UpdateExperience(characterController.levelData);
        foreach (ObjectWithHealth objectWithHealth in objectsWithHealth)
        {
            UpdateHealthBar(
                objectWithHealth.healthState,
                findHpTextInObject(objectWithHealth.gameObject),
                findHealthBarForegroundInObject(objectWithHealth.gameObject)
            );
        }
    }

    public void OpenStatsPanel()
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

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.CHARACTER_LEVEL_UP:
                addStatsIcon.SetActive(true);
                break;
            case EventType.RESET_HEALTH:
                ObjectWithHealth objectToReset = (ObjectWithHealth)eventDTO.eventData;
                SetHealthForObject(objectToReset);
                break;
            case EventType.ENEMY_KILLED:
                GameObject enemy = (GameObject)eventDTO.eventData;
                objectsWithHealth.Remove(enemy.GetComponent<ObjectWithHealth>());
                break;
        }
    }

    private void SetHealthForObject(ObjectWithHealth objectWithHealth)
    {
        HealthState healthState = objectWithHealth.healthState;
        TextMeshProUGUI hpTextField = findHpTextInObject(objectWithHealth.gameObject);
        hpTextField.text = healthState.value + "/" + healthState.maxHealth;
        Image image = findHealthBarForegroundInObject(objectWithHealth.gameObject);
        image.fillAmount = (float)healthState.value / (float)healthState.maxHealth;
    }

    private TextMeshProUGUI findHpTextInObject(GameObject gameObject)
    {
        if (gameObject.tag.Equals(Tags.PLAYER))
        {
            return playerUI.healthText;
        }

        foreach (TextMeshProUGUI textField in gameObject.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (textField.gameObject.name.Equals("HpText"))
            {
                return textField;
            }
        }
        throw new Exception("Not found hp text: " + gameObject);
    }

    internal void HideAddStatsIcon()
    {
        addStatsIcon.SetActive(false);
    }

    internal void AddQuestToUI(Quest quest, int questIndex)
    {
        Transform questPanelTransform = questPanel.gameObject.transform;
        GameObject summaryGameObject = questPanelTransform.GetChild(questIndex * 2).gameObject;
        summaryGameObject.SetActive(true);
        TextMeshProUGUI summaryTextField = questPanelTransform
            .GetChild(questIndex * 2)
            .gameObject.GetComponentInChildren<TextMeshProUGUI>();
        GameObject descriptionGameObject = questPanelTransform
            .GetChild(questIndex * 2 + 1)
            .gameObject;
        descriptionGameObject.SetActive(true);
        TextMeshProUGUI descriptionTextField =
            descriptionGameObject.GetComponent<TextMeshProUGUI>();
        summaryTextField.text = quest.summary;
        descriptionTextField.text = quest.questParts[0];
    }

    internal void RemoveQuestFromUI(Quest quest)
    {
        Transform questPanelTransform = questPanel.gameObject.transform;
        for (int i = 0; i < questPanelTransform.childCount; i = i + 2)
        {
            GameObject summaryGameObject = questPanelTransform.GetChild(i).gameObject;
            string summaryTextValue = summaryGameObject
                .GetComponentInChildren<TextMeshProUGUI>()
                .text;
            if (summaryTextValue.Equals(quest.summary))
            {
                summaryGameObject.SetActive(false);
                questPanelTransform.GetChild(i + 1).gameObject.SetActive(false);
                return;
            }
        }
    }

    public void UpdateDescription(Quest quest, string newValue)
    {
        Transform questPanelTransform = questPanel.gameObject.transform;
        for (int i = 0; i < questPanelTransform.childCount; i = i + 2)
        {
            GameObject questSummary = questPanelTransform.GetChild(i).gameObject;
            TextMeshProUGUI questSummaryText =
                questSummary.GetComponentInChildren<TextMeshProUGUI>();
            if (questSummaryText.text.Equals(quest.summary))
            {
                GameObject descriptionGameObject = questPanelTransform.GetChild(i + 1).gameObject;
                TextMeshProUGUI descriptionTextField =
                    descriptionGameObject.GetComponent<TextMeshProUGUI>();
                descriptionTextField.text = newValue;
                break;
            }
        }
    }

    internal void ChangeDescription(Quest quest, int step)
    {
        Transform questPanelTransform = questPanel.gameObject.transform;
        for (int i = 0; i < questPanelTransform.childCount; i = i + 2)
        {
            GameObject questSummary = questPanelTransform.GetChild(i).gameObject;
            TextMeshProUGUI questSummaryText =
                questSummary.GetComponentInChildren<TextMeshProUGUI>();
            if (questSummaryText.text.Equals(quest.summary))
            {
                GameObject descriptionGameObject = questPanelTransform.GetChild(i + 1).gameObject;
                TextMeshProUGUI descriptionTextField =
                    descriptionGameObject.GetComponent<TextMeshProUGUI>();
                descriptionTextField.text = quest.questParts[step];
                break;
            }
        }
    }
}
