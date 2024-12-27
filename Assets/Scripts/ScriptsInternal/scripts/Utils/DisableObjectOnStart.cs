using UnityEngine;

public class DisableObjectOnStart : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}
