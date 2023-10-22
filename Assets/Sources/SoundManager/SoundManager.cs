using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    AudioSource source;

    void Awake()
    {
        base.Awake();
        source = GetComponent<AudioSource>();
        source = source == null ? gameObject.AddComponent<AudioSource>() : source;

        source.Play();
    }

    public void PlayClip(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}

public static class SounManagerHelper
{
    public static void Play(this AudioClip clip) => SoundManager.Instance.PlayClip(clip);
}
