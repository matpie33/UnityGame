﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EventQueue : MonoBehaviour
{
    private List<Observer> observers = new List<Observer>();

    private void Awake()
    {
        observers = FindObjectsOfType<Observer>().ToList();
    }

    public void SubmitEvent(EventDTO eventDTO)
    {
        observers.RemoveAll(o => o == null);
        foreach (Observer observer in observers)
        {
            observer.OnEvent(eventDTO);
        }
    }
}
