using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Publisher : MonoBehaviour
{
    public abstract void AddObserver(Observer observer);
}
