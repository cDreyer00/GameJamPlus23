using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sources
{
    [Serializable]
    public struct CooldownTimer
    {
        public float cooldownTime;
        float        _time;
        public CooldownTimer(float cooldownTime)
        {
            this.cooldownTime = cooldownTime;
            _time = cooldownTime;
        }
        public void Tick(float deltaTime)
        {
            _time = Mathf.Max(_time - deltaTime, 0);
        }
        public void Reset()
        {
            _time = cooldownTime;
        }
        public static implicit operator float(CooldownTimer cooldown)
        {
            return cooldown._time;
        }
        public static implicit operator bool(CooldownTimer cooldown)
        {
            return cooldown._time <= 0;
        }
        public static implicit operator CooldownTimer(float time)
        {
            return new CooldownTimer(time);
        }
    }
}