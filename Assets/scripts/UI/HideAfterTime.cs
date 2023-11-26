using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAfterTime : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke(nameof(Hide), 2f);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
