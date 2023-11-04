using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeInOut : Observer
{
    private Image image;

    private CanvasRenderer canvasRenderer;

    private void Awake()
    {
        image = GetComponent<Image>();
        canvasRenderer = GetComponent<CanvasRenderer>();
        canvasRenderer.SetAlpha(0);
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        switch (eventDTO.eventType)
        {
            case EventType.CHARACTER_LEVEL_UP:
                image.CrossFadeAlpha(1, 2, false);
                break;
        }
    }

    private void Update()
    {
        float currentAlpha = canvasRenderer.GetAlpha();
        if (currentAlpha == 1)
        {
            image.CrossFadeAlpha(0, 1.2f, true);
        }
    }
}
