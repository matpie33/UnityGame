using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [field: SerializeField]
    public TextMeshProUGUI medipackAmountText { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI healthText { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI experienceText { get; set; }

    [field: SerializeField]
    public Image healthBar { get; set; }

    [field: SerializeField]
    public Image experienceBar { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI levelText { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI strengthStat { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI agilityStat { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI defenceStat { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI healthStat { get; set; }

    [field: SerializeField]
    public GameObject statsPanel { get; set; }

    [field: SerializeField]
    public Button addAgilityButton { get; private set; }

    [field: SerializeField]
    public Button addDefenceButton { get; private set; }

    [field: SerializeField]
    public Button addHealthButton { get; private set; }

    [field: SerializeField]
    public Button addStrengthButton { get; private set; }

    [field: SerializeField]
    public GameObject statsBottomPanel { get; private set; }

    [field: SerializeField]
    public TextMeshProUGUI statsLeftText { get; set; }
}
