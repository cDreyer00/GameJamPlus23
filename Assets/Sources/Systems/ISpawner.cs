using System.Collections;
using UnityEngine;

namespace Sources.Systems
{
    public interface ISpawner<in T>
    {
        IEnumerator SpawnCoroutine();
    }
}