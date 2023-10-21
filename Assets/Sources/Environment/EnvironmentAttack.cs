using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CDreyer;
using Sources.Enemy;
using UnityEngine;

public class EnvironmentAttack : Spawner<EnvAreaAttack>
{
    public override void OnAfterSpawn(EnvAreaAttack instance)
    {
        instance.Init();
    }
}