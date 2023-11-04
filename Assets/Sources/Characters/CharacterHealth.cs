using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : CharacterModule
{
    [SerializeField] ClampedPrimitive<float> _health;
    [SerializeField] Slider _healthSlider;

    void OnEnable()
    {
        Character.Events.onTakeDamage += OnTakeDamage;
    }

    void OnDisable()
    {
        Character.Events.onTakeDamage -= OnTakeDamage;
    }

    void Start()
    {
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