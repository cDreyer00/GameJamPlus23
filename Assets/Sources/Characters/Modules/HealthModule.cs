using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UnityEngine.Events;


public class HealthModule : CharacterModule
{
    [SerializeField] Canvas                  canvas;
    [SerializeField] Slider                  healthSlider;
    [SerializeField] ClampedPrimitive<float> health;
    [SerializeField] AudioClip               hitAudio;
    [SerializeField] AudioClip               dieAudio;
//<<<<<<< HEAD

    public event Action<float> OnTakeDamage = delegate {};
// =======
//     [SerializeField] MMFeedbacks hitFeedback;
//     [SerializeField] AudioClip hitAudio;
//     [SerializeField] AudioClip dieAudio;
//
// >>>>>>> 27b73a94e1852dba9c401135c90eb2ed5c6bae1b
    public float BaseHealth { get; private set; }

    public float Health
    {
        get => health.Value;
        set
        {
            health.Value = value;
            UpdateSlider();
        }
    }

    public float MaxHealth
    {
        get => health.max;
        set
        {
            health.max = value;
            UpdateSlider();
        }
    }

    float audioDelay   = 0.3f;
    bool  playingAudio = false;

    public void OnEnable()
    {
        Character.Events.OnTakeDamage += TakeDamage;
        Character.Events.OnInitialized += Init;
    }

    public void OnDisable()
    {
        Character.Events.OnTakeDamage -= TakeDamage;
        Character.Events.OnInitialized -= Init;
    }

    protected override void Init()
    {
        canvas.worldCamera = Camera.main;

        health.Value = health.max;
        BaseHealth = health.max;
        UpdateSlider();

        playingAudio = false;
    }

    public void TakeDamage(float amount)
    {
//<<<<<<< HEAD
        float damageTaken = health.Value - amount;
        if (damageTaken > 0) OnTakeDamage.Invoke(damageTaken);
        health.Value = damageTaken;
        if(health <= 0 && dieAudio) dieAudio.Play(); 
// =======
//         health.Value -= amount;
//         if (hitFeedback != null) hitFeedback.PlayFeedbacks();
//         if (health <= 0) {
//             Character.Events.Died(Character);
//
//             if (dieAudio != null) dieAudio.Play();
//         }
//
// >>>>>>> 27b73a94e1852dba9c401135c90eb2ed5c6bae1b
        UpdateSlider();

        if (hitAudio != null && !playingAudio) {
            hitAudio.Play();
            playingAudio = true;
            this.Delay(audioDelay, c => c.playingAudio = false);
        }
    }

    void UpdateSlider()
    {
        healthSlider.maxValue = health.max;
        healthSlider.minValue = health.min;
        healthSlider.value = health.Value;
    }
}