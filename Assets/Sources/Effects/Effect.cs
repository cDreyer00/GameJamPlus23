using UnityEngine;

public abstract class Effect
{
    public abstract void ApplyEffect(ICharacter character);
}

public class ConfusionEffect : Effect
{
    public override void ApplyEffect(ICharacter character)
    {
        Debug.Log("confusion effect applied");
    }
}

public class FreezeEffect : Effect
{
    public override void ApplyEffect(ICharacter character)
    {
        Debug.Log("Freeze effect applied");
    }
}