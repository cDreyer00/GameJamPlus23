using System;
using System.Collections;
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
    public static class EffectManager
    {

        public static void ApplyEffect(IEnemy enemy, Effect effect)
        {
            Action<IEnemy> e = effect switch
            {
                Effect.Confusion => ApplyConfusion,
                Effect.Slow => ApplySlow,
            };

            e?.Invoke(enemy);
        }

        static void ApplyConfusion(IEnemy enemy)
        {

        }

        static void ApplySlow(IEnemy enemy)
        {

        }

        // public IEnumerator Confusion(float time)
        // {
        //     foreach (var enemy in EnemySpawner.Instance.Instances)
        //     {
        //         enemy.canMove = false;
        //     }
        //     while (time > 0)
        //     {
        //         foreach (var enemy in EnemySpawner.Instance.Instances)
        //         {
        //             enemy.SetDestination(GetRandomPoint());
        //         }

        //         time -= Time.deltaTime;
        //         yield return null;
        //     }
        //     foreach (var enemy in EnemySpawner.Instance.Instances)
        //     {
        //         enemy.canMove = true;
        //     }
        // }
    }
}