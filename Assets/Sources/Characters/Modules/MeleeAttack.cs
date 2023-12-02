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
        Helpers.DelayFrames(2, static c => {
            c._target = GameManager.Instance.Player;
        }, this);
    }

    void OnEnable()
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
            Helpers.Delay(_delay, Attack);
        }
    }

    void Attack()
    {
        if (_target == null) return;

        float dist = Vector3.Distance(Character.Position, _target.Position);
        if (dist <= _range)
            _target.Events.OnTakeDamage(_damage);

        Helpers.Delay(_coolDown, static c => c._isAttacking = false, this);
    }
}