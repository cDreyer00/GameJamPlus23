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

        public static void ApplyEffect(IEnemy enemy, Effect effect, float timer)
        {
            Action<IEnemy, float> e = effect switch
            {
                Effect.Confusion => ApplyConfusion,
                Effect.Slow => ApplySlow,
                _ => throw new NotImplementedException(),
            };

            e?.Invoke(enemy, timer);
        }

        static void ApplyConfusion(IEnemy enemy, float timer)
        {
            float randX = UnityEngine.Random.Range(-100, 100);
            float randZ = UnityEngine.Random.Range(-100, 100);
            enemy.SetDestForTimer(new(randX, enemy.Pos.y, randZ), timer);
        }

        static void ApplySlow(IEnemy enemy, float timer)
        {
            enemy.SetSpeed(1, timer);
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