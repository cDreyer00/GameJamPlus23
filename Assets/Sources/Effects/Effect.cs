using Sources.Characters;
using UnityEngine;

namespace Sources.Effects
{
    public abstract class BaseEffect : MonoBehaviour
    {
        public abstract void ApplyEffect(Character character);
    }
    public class ConfusionEffect : BaseEffect
    {
        public override void ApplyEffect(Character character) {}
    }
    public class FreezeEffect : BaseEffect
    {
        public override void ApplyEffect(Character character) {}
    }
}