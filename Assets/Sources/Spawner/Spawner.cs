using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Spawner : MonoBehaviour
{
    [SerializeField] Battle battle;
    [SerializeField] float elapsedTime;

    Queue<Wave> wavesQueue;
    Wave curWave;
    float curWaveDuration;
    float curwaveElapseTime;

    public void Init()
    {
        wavesQueue = new(battle.Waves);
        NextWave();
        elapsedTime = 0;
    }

    void Update()
    {
        if (wavesQueue == null) return;
        if (wavesQueue.Count == 0) return;

        elapsedTime += Time.deltaTime;
        curwaveElapseTime += Time.deltaTime;
        curWaveDuration -= Time.deltaTime;


        if (curWaveDuration <= 0)
            NextWave();
    }

    void Spawn()
    {
        int amount = curWave.GetAmount(elapsedTime);
    }

    int GetAmountToSpawn()
    {
        if (curWave == null) return 0;
        return curWave.GetAmount(elapsedTime);
    }

    void NextWave()
    {
        curWave = wavesQueue.Dequeue();
        curWaveDuration = curWave.Duration;
        curwaveElapseTime = 0;
    }
}