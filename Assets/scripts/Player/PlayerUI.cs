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
}
