using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIUpdater : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI medipackAmount;

    public void UpdateMedipackAmount(int newValue)
    {
        medipackAmount.text = "" + newValue;
    }
}
