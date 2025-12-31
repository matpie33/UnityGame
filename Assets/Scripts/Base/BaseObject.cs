using System;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    [SerializeField]
    private string uuid;

    public String GetUUid()
    {
        return uuid;
    }

    public void SetUuid(String value)
    {
        if (this.uuid.Length == 0)
        {
            this.uuid = value;
        }
    }
}
