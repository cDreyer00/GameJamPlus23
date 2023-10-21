using System;
using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : Singleton<PowerBar>
{
    [SerializeField] Slider slider;
    [SerializeField] float decaySpeed = 0.1f;
    [SerializeField] float decayAmount = 1f;
    [SerializeField] float maxPower = 100f;
    [SerializeField] float power = 0;
    [SerializeField] int powerLevel = 0;

    public event Action<PowerBarEventArgs> onPowerChanged;

    public int DiscretePowerLevel()
    {
        return power switch
        {
            < 15 => 0,
            < 40 => 1,
            < 60 => 2,
            < 80 => 3,
            < 95 => 4,
            _ => 5
        };
    }

    public IEnumerator DecayPower()
    {
        while (true)
        {
            yield return Helpers.GetWait(decaySpeed);
            UpdatePower(-decayAmount);
        }
    }

    private void Start()
    {
        slider.maxValue = maxPower;
        slider.value = power;
        StartCoroutine(DecayPower());
    }

    public void UpdatePower(float amount)
    {
        power += amount;
        power = Math.Clamp(power, 0, maxPower);
        slider.value = power;
        powerLevel = DiscretePowerLevel();

        onPowerChanged?.Invoke(new(power, powerLevel));
    }
}

public class PowerBarEventArgs
{
    public float Power { get; }
    public int PowerLevel { get; }

    public PowerBarEventArgs(float power, int powerLevel)
    {
        Power = power;
        PowerLevel = powerLevel;
    }
}
