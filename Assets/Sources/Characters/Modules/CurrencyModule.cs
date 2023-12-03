using UnityEngine;

public class CurrencyModule : CharacterModule
{
    [SerializeField] int currencyOnDie;

    protected override void Init()
    {
        Character.Events.OnDied += (_) => Progress.Instance.currency.money += currencyOnDie;
    }
}