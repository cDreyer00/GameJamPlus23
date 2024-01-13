using System;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    bool soundEnabled = true;
    public bool SoundEnabled
    {
        get => soundEnabled;
        set
        {
            soundEnabled = value;
            OnSoundEnabled?.Invoke(soundEnabled);
        }
    }

    public event Action<bool> OnSoundEnabled;

    AudioSource source;

    protected override void Awake()
    {
        base.Awake();
        source = GetComponent<AudioSource>();
        source = source == null ? gameObject.AddComponent<AudioSource>() : source;

        if (Instance == this)
            source.Play();
    }

    public void PlayClip(AudioClip clip)
    {
        if (!soundEnabled)
            return;

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
    public static void Play(this AudioClip clip) => SoundManager.Instance.PlayClip(clip);
}