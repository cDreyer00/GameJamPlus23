using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class EnvAreaAttack : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float range;
    [SerializeField] float timer;

    public event Action<EnvAreaAttack> onExplode;

    public void Init()
    {
        Helpers.ActionCallback(DealDamage, timer);
        transform.localScale = Vector3.one * range;
    }

    public void DealDamage()
    {
        IPlayer player = GameManager.Instance.Player;
        if (Vector3.Distance(player.Pos, transform.position) < range)
            player.TakeDamage((int)damage);

        onExplode?.Invoke(this);
        Destroy(gameObject);
    }
}