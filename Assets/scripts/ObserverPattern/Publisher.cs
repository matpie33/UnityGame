using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Publisher : MonoBehaviour
{
    protected List<Observer> observers = new List<Observer>();

    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }
}
