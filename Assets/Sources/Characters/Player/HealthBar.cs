using System;
using Sources.cdreyer;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : Singleton<HealthBar>
{
    [SerializeField] Slider slider;
    [SerializeField] float maxHealthPoints;
    [SerializeField] float healthPoints;

    public float HealthPoints => healthPoints;
    public float MaxHealthPoints => maxHealthPoints;
    private void Start()
    {
        slider.maxValue = maxHealthPoints;
        slider.value = maxHealthPoints - healthPoints;
    }
    public void Damage(float amount)
    {
        healthPoints -= amount;
        healthPoints = Math.Clamp(healthPoints, 0, maxHealthPoints);
        slider.value = maxHealthPoints - healthPoints; // HACK: Slider is inverted
    }
}