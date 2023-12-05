using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
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