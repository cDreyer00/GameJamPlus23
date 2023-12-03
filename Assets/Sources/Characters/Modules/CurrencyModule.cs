using UnityEngine;

public class CurrencyModule : CharacterModule
{
    [SerializeField] int currencyOnDie;

    protected override void Init()
    {
        Character.Events.Died += (_) => Progress.Instance.currency.money += currencyOnDie;
    }
}