using Sources.Characters;
using UnityEngine;

public abstract class BaseEffect : MonoBehaviour
{
    public abstract void ApplyEffect(Character character);
}
public class ConfusionEffect : BaseEffect
{
    public override void ApplyEffect(Character character) { }
}
public class FreezeEffect : BaseEffect
{
    public override void ApplyEffect(Character character) { }
}