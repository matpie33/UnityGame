using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LevelUpEffectTrigger : Observer
{
    private VisualEffect visualEffect;

    private void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
    }

    public override void OnEvent(EventDTO eventDTO)
    {
        if (eventDTO.eventType.Equals(EventType.CHARACTER_LEVEL_UP))
        {
            visualEffect.Play();
        }
    }
}
