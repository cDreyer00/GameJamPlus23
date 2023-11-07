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

    void OnEnable()
    {
        Character.Events.onTakeDamage += OnTakeDamage;
        Character.Events.onInitialized += Init;
    }

    void OnDisable()
    {
        Character.Events.onTakeDamage -= OnTakeDamage;
        Character.Events.onInitialized -= Init;
    }

    protected override void Init()
    {
        canvas.worldCamera = Camera.main;

        health.Value = health.max;
        UpdateSlider();
    }


    void OnTakeDamage(float amount)
    {
        Character.ReferenceCount++;
        health.Value -= amount;

        if (health <= 0)
            Character.Events.onDied?.Invoke(Character);

        UpdateSlider();
        Character.ReferenceCount--;
    }

    void UpdateSlider()
    {
        healthSlider.maxValue = health.max;
        healthSlider.minValue = health.min;
        healthSlider.value = health.Value;
    }
}