using System.Collections;
using UnityEngine;

public class FeedbackDamage : CharacterModule
{
    [SerializeField] Color color, color2;
    [SerializeField] float duration;
    [SerializeField] bool is3D;

    SpriteRenderer _sprite;

    protected override void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        Character.Events.onTakeDamage += DamageColor;
        Character.Events.freeze += Freeze;
    }

    void OnEnable()
    {
        if (_sprite != null)
            _sprite.color = color2;
    }

    void DamageColor(float dmgAmount)
    {
        if (!is3D)
        {
            _sprite.color = color;
        }
        else
            print("Hit Damage");

        Helpers.Delay(duration, () => _sprite.color = color2);
    }


    void Freeze(float duration)
    {
        Debug.Log("freeze feedback");
        _sprite.color = Color.cyan;
        Helpers.Delay(duration, () => _sprite.color = color2);
    }

}