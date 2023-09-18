using UnityEditor;
using UnityEngine;

public class Medkit : Pickable
{
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public override void Interact(Object data)
    {
        GameObject rightHandObject = (GameObject)data;
        GetComponent<BoxCollider>().enabled = false;
        transform.SetParent(rightHandObject.transform);
        transform.localPosition = new Vector3(0, 0, 0);
    }
}
