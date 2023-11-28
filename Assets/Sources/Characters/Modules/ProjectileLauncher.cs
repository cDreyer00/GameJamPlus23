using System;
using Sources.Characters.Modules;
using UnityEngine;
using static Character.State;

public class ProjectileLauncher : CharacterModule
{
    QueuePool<Projectile> _projectilePool;

    [SerializeField] Projectile projectile;
    [SerializeField] Transform  target;
    [SerializeField] float      delay;
    [SerializeField] float      coolDown;
    public Transform Target
    {
        get => target;
        set => target = value;
    }
    protected override void Init()
    {
        _projectilePool = new QueuePool<Projectile>(projectile, 10);
    }
    public override void StartModule()
    {
        InvokeRepeating(nameof(Shoot), delay, coolDown);
    }
    public override void StopModule()
    {
        CancelInvoke(nameof(Shoot));
    }
    void Shoot()
    {
        var position = target.position;
        var dir      = (position - Character.Position).normalized;
        var p        = _projectilePool.Get(Character.Position, Quaternion.LookRotation(dir));
        p.IgnoreTeam(Character.team);
        p.target = position;
    }
}