using UnityEngine;

public abstract class Effect
{
    public float damage   = 0;
    public float duration = 0;

    public abstract void ApplyEffect(Character character);
}

public class ConfusionEffect : Effect
{
    public override void ApplyEffect(Character character)
    {
        Debug.Log("confusion effect applied");
    }
}

public class FreezeEffect : Effect
{
    public override void ApplyEffect(Character character)
    {
        character.Events.OnFreeze(duration);
    }
}

public class DamageEffect : Effect
{
    public override void ApplyEffect(Character character)
    {
        character.Events.OnTakeDamage(damage);
    }
}