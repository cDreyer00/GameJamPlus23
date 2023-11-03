using System.Collections;
using UnityEngine;

public class FeedbackDamage : MonoBehaviour
{
    [SerializeField] Color color, color2;

    SpriteRenderer _sprite;
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }
    public IEnumerator DamageColor()
    {
        _sprite.color = color;
        yield return new WaitForSeconds(.5f);
        _sprite.color = color2;
    }
}