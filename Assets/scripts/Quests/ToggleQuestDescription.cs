using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleQuestDescription : MonoBehaviour
{
    [SerializeField]
    private GameObject description;

    public void ToggleDescriptionVisiblity()
    {
        description.SetActive(!description.activeSelf);
    }
}
