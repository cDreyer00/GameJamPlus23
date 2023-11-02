using System;
using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PowerBar : Singleton<PowerBar>
{
    [SerializeField] Slider slider;
    [SerializeField] float decaySpeed = 0.1f;
    [SerializeField] float decayAmount = 1f;
    [SerializeField] float maxPower = 100f;
    [SerializeField] float power;
    [SerializeField] float extraDecayMod = 1.1f;
    [SerializeField] int powerLevel;
    [SerializeField] private float[] powerTrashHolds;
    public float Power => power;
    public float MaxPower => maxPower;
    public event Action<PowerBarEventArgs> onPowerChanged;

    public int DiscretePowerLevel()
    {
        for (int i = 0; i < powerTrashHolds.Length; i++)
        {
            if (power >= powerTrashHolds[i])
                return i;
        }

        return powerTrashHolds.Length;
    }

    public IEnumerator DecayPower()
    {
        while (true)
        {
            yield return Helpers.GetWait(decaySpeed);
            UpdatePower(-decayAmount * (extraDecayMod * powerLevel));
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
        power -= amount;
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