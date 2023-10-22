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
    }

    public void PlayClip(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}
