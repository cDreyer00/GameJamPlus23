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
    
    public void AddPower(float amount)
    {
        slider.value += amount;
    }
}