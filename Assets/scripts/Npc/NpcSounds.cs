using System;
using System.Collections;
using UnityEngine;

public class NpcSounds : MonoBehaviour
{
    [SerializeField]
    private AudioSource helloMessage;

    [SerializeField]
    private AudioSource letsMoveMessage;

    [SerializeField]
    private AudioSource underAttackMessage;

    [SerializeField]
    private AudioSource weReSafeMessage;

    public float PlayHelloMessage()
    {
        helloMessage.Play();
        return helloMessage.clip.length;
    }

    internal void PlayLetsMove()
    {
        letsMoveMessage.Play();
    }

    internal void PlayUnderAttack()
    {
        underAttackMessage.Play();
    }

    internal void PlayWeReSafe()
    {
        weReSafeMessage.Play();
    }
}
