using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CDreyer;
using UnityEngine;

public class EnvironmentAttack : MonoBehaviour
{
    [SerializeField] Vector2 xRange;
    [SerializeField] Vector2 zRange;

    [SerializeField] float damage = 1;
    [SerializeField] float delay = 5;

    [SerializeField] float attackRange = 5;
    [SerializeField] float attackTimer = 3;

    float curDelay = 3;

    void Update()
    {
        curDelay -= Time.deltaTime;
        if (curDelay <= 0)
        {
            Attack();
            curDelay = delay;
        }

    }

    public void Attack()
    {
        int randX = Random.Range((int)xRange.x, (int)xRange.y);
        int randZ = Random.Range((int)zRange.x, (int)zRange.y);

        var pos = new Vector3(randX, 0, randZ);
        attack = new(damage, attackRange, attackTimer, pos);
        GameLogger.Log($"new attack created at pos {attack.Pos}");
        attack.Init();
    }

    EnvAreaAttack attack;
    void OnDrawGizmos()
    {
        if (attack == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(attack.Pos, attackRange);
    }
}

public class EnvAreaAttack
{
    public float Damage { get; }
    public float Range { get; }
    public float Timer { get; }
    public Vector3 Pos { get; }

    public EnvAreaAttack(float damage, float range, float timer, Vector3 pos)
    {
        this.Damage = damage;
        this.Range = range;
        this.Timer = timer;
        this.Pos = pos;
    }

    public void Init()
    {
        Helpers.ActionCallback(DealDamage, Timer);
    }

    public void DealDamage()
    {
        IPlayer player = null;
        Physics.OverlapSphere(Pos, Range).FirstOrDefault(c => c.TryGetComponent(out player));
        player?.TakeDamage((int)Damage);
    }
}