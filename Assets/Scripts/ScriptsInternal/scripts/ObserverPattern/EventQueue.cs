using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventQueue : MonoBehaviour
{
    private List<Observer> observers = new List<Observer>();

    private void Awake()
    {
        observers = FindObjectsByType<Observer>(FindObjectsSortMode.None).ToList();
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
