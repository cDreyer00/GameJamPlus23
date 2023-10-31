using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    AudioSource source;

    protected override void Awake()
    {
        base.Awake();
        source = GetComponent<AudioSource>();
        source = source == null ? gameObject.AddComponent<AudioSource>() : source;

        if (PlayerHealthBar == this)
            source.Play();
    }

    public void PlayClip(AudioClip clip)
    {
        source.PlayOneShot(clip);

    }
    public void Stop()
    {
        source.Stop();
    }

    void OnDisable()
    {
        if (source != null)
            source.Stop();
    }
}

public static class SounManagerHelper
{
    public static void Play(this AudioClip clip) => SoundManager.PlayerHealthBar.PlayClip(clip);
}
