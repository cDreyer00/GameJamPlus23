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
    
    public event Action<float> OnTakeDamage = delegate { };
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
    }

    public void TakeDamage(float amount)
    {
        float damageTaken = health.Value - amount;
        if(damageTaken > 0) OnTakeDamage.Invoke(damageTaken);
        health.Value = damageTaken;
        UpdateSlider();
    }

    void UpdateSlider()
    {
        healthSlider.maxValue = health.max;
        healthSlider.minValue = health.min;
        healthSlider.value = health.Value;
    }
}