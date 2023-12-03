using System.Collections;
using UnityEngine;

public class FeedbackDamage : CharacterModule
{
    [SerializeField] Color color, color2;
    [SerializeField] float duration;

    //SpriteRenderer _sprite;

    protected override void Init()
    {
       /* _sprite = GetComponent<SpriteRenderer>();
        Character.Events.TakeDamage += DamageColor;*/
        Character.Events.OnFreeze += Freeze;
    }

   /* void OnEnable()
    {
        if (_sprite != null)
            _sprite.color = color2;
    }*/

   /* void DamageColor(float dmgAmount)
    {
        _sprite.color = color;
        Helpers.Delay(duration, static c => c._sprite.color = c.color2, this);
    }*/


    void Freeze(float duration)
    {
        Debug.Log("freeze feedback");
        //_sprite.color = Color.cyan;
        //Helpers.Delay(duration, static c => c._sprite.color = c.color2, this);
    }
}