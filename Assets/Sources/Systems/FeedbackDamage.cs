using System.Collections;
using UnityEngine;

public class FeedbackDamage : CharacterModule
{
    [SerializeField] Color color, color2;

    SpriteRenderer _sprite;

    protected override void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        Character.Events.onTakeDamage += (amount) => StartCoroutine(DamageColor());
        Character.Events.freeze += Freeze;
    }

    void OnEnable()
    {
        if (_sprite != null)
            _sprite.color = color2;
    }

    public IEnumerator DamageColor()
    {
        _sprite.color = color;
        yield return new WaitForSeconds(.5f);
        _sprite.color = color2;
    }


    void Freeze(float duration)
    {
        Debug.Log("freeze feedback");
        _sprite.color = Color.cyan;
        Helpers.Delay(duration, () => _sprite.color = color2);
    }

}