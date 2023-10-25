using System;
using System.Collections;
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

    Animator _animator;
    public event Action<EnvAreaAttack> onExplode;

    public void Init()
    {
        _animator = GetComponentInChildren<Animator>();
        StartCoroutine(DealDamage());
    }

    private void Update()
    {
        transform.localScale = Vector3.one * radius;
        timer -= Time.deltaTime;
    }

    public IEnumerator DealDamage()
    {
        if (this.IsDestroyed()) yield break;
        const int Layer     = 0; // Base Layer
        var       clipInfos = _animator.GetCurrentAnimatorClipInfo(Layer);
        var       clipInfo  = clipInfos[0];

        while (clipInfo.clip.name != "Attack") {
            yield return null;
            clipInfos = _animator.GetCurrentAnimatorClipInfo(Layer);
            clipInfo  = clipInfos[0];
        }

        IPlayer player = GameManager.Instance.Player;
        if (Vector3.Distance(player.Pos, transform.position) < radius) {
            player.TakeDamage((int)damage);
        }

        while (clipInfo.clip.name != "Destroy") {
            yield return null;
            clipInfos = _animator.GetCurrentAnimatorClipInfo(Layer);
            clipInfo  = clipInfos[0];
        }

        onExplode?.Invoke(this);
        Destroy(gameObject);
    }
}