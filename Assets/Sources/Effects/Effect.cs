using UnityEngine;

public abstract class Effect
{
    public abstract void ApplyEffect(ICharacter character);
}

public class ConfusionEffect : Effect
{
    public override void ApplyEffect(ICharacter character)
    {
        // character.Movement.SetDirection(Vector3.one);
    }
}

public class FreezeEffect : Effect
{
    public override void ApplyEffect(ICharacter character)
    {

    }
}