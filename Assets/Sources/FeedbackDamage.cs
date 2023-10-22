using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackDamage : MonoBehaviour
{
    [SerializeField] Color color, color2;

    public IEnumerator DamageColor()
    {
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(.5f);
        GetComponent<SpriteRenderer>().color = color2;
    }
}
