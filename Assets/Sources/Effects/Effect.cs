public abstract class Effect
{
    public abstract void ApplyEffect(IEnemy character);
}

public class ConfusionEffect : Effect
{
    public override void ApplyEffect(IEnemy character) { 
        character.TakeDamage(1000);
    }
}

public class FreezeEffect : Effect
{
    public override void ApplyEffect(IEnemy character) { 

    }
}