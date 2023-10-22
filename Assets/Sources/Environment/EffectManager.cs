using System.Collections;
using System.Collections.Generic;
using Sources.Enemy;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Sources.Environment
{
    public enum Effect
    {
        Confusion,
        Slow
    }

    public sealed class EffectManager : MonoBehaviour
    {
        [SerializeField] private MeshRenderer mr;

        private Vector3 GetRandomPoint()
        {
            var bounds = mr.bounds;
            float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
            float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);
            return new Vector3(x, 0, z);
        }

        public IEnumerator Confusion(float time)
        {
            foreach (var enemy in EnemySpawner.Instance.Instances)
            {
                enemy.canMove = false;
            }

            while (time > 0)
            {
                foreach (var enemy in EnemySpawner.Instance.Instances)
                {
                    enemy.SetDestination(GetRandomPoint());
                }

                time -= Time.deltaTime;
                yield return null;
            }

            foreach (var enemy in EnemySpawner.Instance.Instances)
            {
                enemy.canMove = true;
            }
        }

        public IEnumerator Slow(float time)
        {
            var speeds = new List<float>();
            foreach (var enemy in EnemySpawner.Instance.Instances)
            {
                speeds.Add(enemy.Speed);
                enemy.Speed = 0;
            }

            yield return Helpers.GetWait(time);

            for (int i = 0; i < EnemySpawner.Instance.Instances.Count; i++)
            {
                var enemy = EnemySpawner.Instance.Instances[i];
                enemy.Speed = speeds[i];
            }
        }
    }
}