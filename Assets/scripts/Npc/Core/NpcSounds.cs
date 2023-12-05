using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSounds : MonoBehaviour
{
    [SerializeField]
    private List<AudioSource> messages;

    private int nextMessageIndex = 0;

    private AudioSource currentlyPlayedClip;

    public void ResetSoundsCounter()
    {
        nextMessageIndex = 0;
    }

    public void SetMessageIndex(int index)
    {
        nextMessageIndex = index;
    }

    public float PlayNextMessage()
    {
        if (nextMessageIndex >= messages.Count)
        {
            throw new Exception("No more sounds to play");
        }
        AudioSource audioSource = messages[nextMessageIndex];
        float length = audioSource.clip.length;
        PlayClip(audioSource);
        nextMessageIndex++;
        return length;
    }

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
}
