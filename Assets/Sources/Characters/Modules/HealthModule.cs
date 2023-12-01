using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthModule : CharacterModule
{
    [SerializeField] Canvas                  canvas;
    [SerializeField] Slider                  healthSlider;
    [SerializeField] ClampedPrimitive<float> health;

    public float Health => health.Value;
    public void OnEnable()
    {
        Character.Events.TakeDamage += OnTakeDamage;
        Character.Events.Initialized += Init;
    }

    public void OnDisable()
    {
        Character.Events.TakeDamage -= OnTakeDamage;
        Character.Events.Initialized -= Init;
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

        if (health <= 0) Character.Events.OnDied(Character);

        UpdateSlider();
    }

    void UpdateSlider()
    {
        healthSlider.maxValue = health.max;
        healthSlider.minValue = health.min;
        healthSlider.value = health.Value;
    }
}