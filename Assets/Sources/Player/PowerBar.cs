using System;
using System.Collections;
using System.Collections.Generic;
using CDreyer;
using UnityEngine;
using Unity.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable IteratorNeverReturns

public class PowerBar : Singleton<PowerBar>
{
    public Slider slider;
    public float decaySpeed = 0.1f;
    public float decayAmount = 1f;
    public float maxPower = 100f;
    public float power;
    public int powerLevel;

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
            slider.value -= decayAmount;
        }
    }

    private void Start()
    {
        slider.maxValue = maxPower;
        slider.value = power;
        StartCoroutine(DecayPower());
    }

    private void Update()
    {
        power = slider.value;
        powerLevel = DiscretePowerLevel();
    }

    public void AddPower(float amount)
    {
        slider.value += amount;
    }
}