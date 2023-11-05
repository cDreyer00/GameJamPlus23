using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthModule : CharacterModule
{
    [SerializeField] Canvas canvas;
    [SerializeField] Slider _healthSlider;
    [SerializeField] ClampedPrimitive<float> _health;

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

        _health.Value = _health.max;
        UpdateSlider();
    }


    void OnTakeDamage(float amount)
    {
        _health.Value -= amount;

        if (_health <= 0)
            Character.Events.onDied?.Invoke(Character);

        UpdateSlider();
    }

    void UpdateSlider()
    {
        _healthSlider.maxValue = _health.max;
        _healthSlider.minValue = _health.min;
        _healthSlider.value = _health.Value;
    }
}