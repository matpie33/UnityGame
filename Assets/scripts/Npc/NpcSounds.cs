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

    private AudioSource currentlyPlayedClip;

    public void StopCurrentClip()
    {
        if (currentlyPlayedClip != null)
        {
            currentlyPlayedClip.Stop();
        }
    }

    private void PlayClip(AudioSource audioSource)
    {
        currentlyPlayedClip = audioSource;
        currentlyPlayedClip.Play();
        Invoke(nameof(ClearCurrentClip), audioSource.clip.length);
    }

    private void ClearCurrentClip()
    {
        currentlyPlayedClip = null;
    }

    public float PlayHelloMessage()
    {
        PlayClip(helloMessage);
        return helloMessage.clip.length;
    }

    internal void PlayLetsMove()
    {
        PlayClip(letsMoveMessage);
    }

    internal void PlayUnderAttack()
    {
        PlayClip(underAttackMessage);
    }

    internal void PlayWeReSafe()
    {
        PlayClip(weReSafeMessage);
    }
}
