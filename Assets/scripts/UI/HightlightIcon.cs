using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HighlightIcon : MonoBehaviour
{
    [SerializeField]
    private Color normal;

    [SerializeField]
    private Color green;

    private Image image;

    private bool switchToGreen = true;

    [SerializeField]
    private float interval;

    [SerializeField]
    private float lerpValue;

    private void Start()
    {
        image = GetComponent<Image>();
        InvokeRepeating(nameof(ChangeColor), interval, interval);
    }

    private void Update()
    {
        image.color = Color.Lerp(image.color, switchToGreen ? green : normal, lerpValue);
    }

    private void ChangeColor()
    {
        switchToGreen = !switchToGreen;
    }
}
