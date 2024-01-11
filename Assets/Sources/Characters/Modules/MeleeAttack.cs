using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class MeleeAttack : CharacterModule
{
    [SerializeField] float _damage;
    [SerializeField] float _range;
    [SerializeField] float _delay;
    [SerializeField] float _coolDown;

    Character   _target;
    public bool isAttacking;

    protected override void Init()
    {
        Helpers.DelayFrames(2, static c => {
            c._target = GameManager.Instance.Player;
        }, this);
    }

    void OnEnable()
    {
        isAttacking = false;
    }

    void Update()
    {
        if (_target == null) return;
        if (isAttacking) return;

        float dist = Vector3.Distance(Character.Position, _target.Position);
        if (dist <= _range) {
            isAttacking = true;
            Helpers.Delay(_delay, Attack);
        }
    }

    void Attack()
    {
        if (_target == null) return;

        float dist = Vector3.Distance(Character.Position, _target.Position);
        if (dist <= _range)
            _target.Events.TakeDamage(_damage);

        this.Delay(_coolDown, static c => c.isAttacking = false);
    }
}