using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;

public class EffectSignSpawner : RampingSpawner<EffectSign>
{
    public override Vector3 GetRandomPosition() => surface.GetRandomPoint();

    protected override void OnSpawned(EffectSign instance)
    {
        instance.Init();
    }
}