using System.Collections;
using UnityEngine;

public class FeedbackDamage : CharacterModule
{
    [SerializeField] Color color, color2;

    SpriteRenderer _sprite;

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    protected override void Init()
    {
        Character.Events.onTakeDamage += (amount) => StartCoroutine(DamageColor());
    }

    public IEnumerator DamageColor()
    {
        _sprite.color = color;
        yield return new WaitForSeconds(.5f);
        _sprite.color = color2;
    }


}