using System;
using System.Collections;
using UnityEngine;

namespace Sources.Enemy
{
    public interface ISpawner<in T>
    {
        Vector3 GetRandomPosition();
        IEnumerator SpawnCoroutine();
    }
}