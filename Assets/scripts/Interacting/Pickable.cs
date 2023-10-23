using UnityEditor;
using UnityEngine;

public class Pickable : Interactable
{
    private CharacterController characterController;

    [field: SerializeField]
    public PickableDefinition definition { get; private set; }

    private void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
    }

    public override void Interact(Object data)
    {
        characterController.ObjectPicked(this);

        GameObject rightHandObject = (GameObject)data;
        GetComponent<Collider>().enabled = false;
        transform.SetParent(rightHandObject.transform);
        transform.localPosition = new Vector3(0, 0, 0);
    }
}
