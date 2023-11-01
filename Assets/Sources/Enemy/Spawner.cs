using UnityEngine;
using Sources.Types;
using Unity.AI.Navigation;

namespace Sources.Enemy
{
    public abstract class RampingSpawner<T> : BaseSpawner<T>
        where T : MonoBehaviour
    {
        [SerializeField] protected NavMeshSurface surface;
        public float DifficultyMod => GameManager.Instance.GameElapsedTime / rampPeriod;
        public float rampPeriod = 10;
        virtual protected void Update()
        {
            float spike = DifficultyMod;
            spawnRate.Value = 1 - spike / 10;
            maxInstances.Value = (int)Mathf.Lerp(maxInstances.Value, maxInstances.max, spike / 10);
        }
    }
}