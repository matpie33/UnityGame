using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Interactable
{
    [field: SerializeField]
    public PickableDefinition requiredKey { get; private set; }

    private CharacterController characterController;

    private Animator animator;

    public bool isOpened { get; set; }

    private void Start()
    {
        characterController = FindObjectOfType<CharacterController>();
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
