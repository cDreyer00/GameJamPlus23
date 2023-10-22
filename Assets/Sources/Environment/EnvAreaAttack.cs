using System;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EnvAreaAttack : MonoBehaviour
{
    public float radius;
    public float damage;
    public float timer;

    public event Action<EnvAreaAttack> onExplode;

    public void Init()
    {
        Helpers.ActionCallback(DealDamage, timer);
        transform.localScale = Vector3.one * radius;
    }

    public void DealDamage()
    {
        if (this.IsDestroyed()) return;

        IPlayer player = GameManager.Instance.Player;
        if (Vector3.Distance(player.Pos, transform.position) < radius)
        {
            player.TakeDamage((int)damage);
        }

        onExplode?.Invoke(this);
        Destroy(gameObject);
    }
}