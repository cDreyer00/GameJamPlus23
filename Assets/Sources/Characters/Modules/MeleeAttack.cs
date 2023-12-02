using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeleeAttack : CharacterModule
{
    [SerializeField] float _damage;
    [SerializeField] float _range;
    [SerializeField] float _delay;
    [SerializeField] float _coolDown;

    Character _target;
    bool      _isAttacking;

    protected override void Init()
    {
        this.DelayFrames(2, static c => {
            c._target = GameManager.Instance.Player;
            Debug.Log("player set as enemy");
        });
    }

    public virtual void StartModule()
    {
        _isAttacking = false;
    }

    void Update()
    {
        if (_target == null) return;
        if (_isAttacking) return;

        float dist = Vector3.Distance(Character.Position, _target.Position);
        if (dist <= _range) {
            _isAttacking = true;
            this.Delay(_delay, Attack);
        }
    }

    static void Attack(MeleeAttack c)
    {
        if (c._target == null) return;

        float dist = Vector3.Distance(c.Character.Position, c._target.Position);
        if (dist <= c._range)
            c._target.Events.OnTakeDamage(c._damage);

        c.Delay(c._coolDown, static c => c._isAttacking = false);
    }
}