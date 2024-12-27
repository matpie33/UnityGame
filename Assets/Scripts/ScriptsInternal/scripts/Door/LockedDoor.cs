using UnityEngine;

public class LockedDoor : Interactable
{
    [field: SerializeField]
    public GameObject requiredKey { get; private set; }

    [field: SerializeField]
    public Transform lockTransform { get; private set; }

    private CharacterController characterController;

    private Animator animator;

    public bool isOpened { get; set; }

    private void Start()
    {
        characterController = FindAnyObjectByType<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public override void Interact(UnityEngine.Object data)
    {
        animator.Play("Base Layer.OpenDoor");
    }

    internal bool PlayerHasKey()
    {
        return characterController.playerBackpack.HasObject(requiredKey);
    }
}
