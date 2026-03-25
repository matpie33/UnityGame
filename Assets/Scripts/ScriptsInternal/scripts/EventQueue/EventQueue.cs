using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventQueue : MonoBehaviour
{
    private List<Observer> observers = new List<Observer>();

    public static EventQueue INSTANCE = null;

    private void Awake()
    {
        observers = FindObjectsByType<Observer>().ToList();
        INSTANCE = this;
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
