using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class HealthModule : CharacterModule
{
    [SerializeField] Canvas canvas;
    [SerializeField] Slider healthSlider;
    [SerializeField] ClampedPrimitive<float> health;
    [SerializeField] MMFeedbacks hitFeedback;

    public float Health => health.Value;
    public void OnEnable()
    {
        Character.Events.OnTakeDamage += OnTakeDamage;
        Character.Events.OnInitialized += Init;
    }

    public void OnDisable()
    {
        Character.Events.OnTakeDamage -= OnTakeDamage;
        Character.Events.OnInitialized -= Init;
    }

    protected override void Init()
    {
        canvas.worldCamera = Camera.main;

        health.Value = health.max;
        UpdateSlider();
    }


    void OnTakeDamage(float amount)
    {
        health.Value -= amount;
        if (hitFeedback != null) hitFeedback.PlayFeedbacks();
        if (health <= 0) Character.Events.Died(Character);

        UpdateSlider();
    }

    void UpdateSlider()
    {
        healthSlider.maxValue = health.max;
        healthSlider.minValue = health.min;
        healthSlider.value = health.Value;
    }
}