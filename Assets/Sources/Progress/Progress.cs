using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sources.cdreyer;
using Sources.cdreyer.SaveSystem;
using System;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "Progress")]
public class Progress : SingletonSO<Progress>, ISavable
{
    [Serializable]
    public class Upgrades
    {
        public enum Type { Health, Damage, Recoil, Barking, AttackSpeed }
        public ClampedPrimitive<int> healthLevel = new(0, 0, 10);
        public ClampedPrimitive<int> damageLevel = new(0, 0, 10);
        public ClampedPrimitive<int> recoilLevel = new(0, 0, 10);
        public ClampedPrimitive<int> brakingLevel = new(0, 0, 10);
        public ClampedPrimitive<int> attackSpeedLevel = new(0, 0, 10);

        public static string FileName => "upgrades.save";

        float mod => 0.2f;

        public void Upgrade(Type upgradeType)
        {
            switch (upgradeType)
            {
                case Type.Health:
                    healthLevel.Value++;
                    break;
                case Type.Damage:
                    damageLevel.Value++;
                    break;
                case Type.Recoil:
                    recoilLevel.Value++;
                    break;
                case Type.Barking:
                    brakingLevel.Value++;
                    break;
                case Type.AttackSpeed:
                    attackSpeedLevel.Value++;
                    break;
            }
        }

        public int GetLevel(Type upgradeType)
        {
            return upgradeType switch
            {
                Type.Health => healthLevel,
                Type.Damage => damageLevel,
                Type.Recoil => recoilLevel,
                Type.Barking => brakingLevel,
                Type.AttackSpeed => attackSpeedLevel,
                _ => 0
            };
        }

        public float GetModValue(float baseValue, Type upgradeType, float mod = 0)
        {
            mod = mod == 0 ? this.mod : mod;
            return baseValue + mod * GetLevel(upgradeType);
        }
    }

    [Serializable]
    public class Currency
    {
        public int money;

        public static string FileName => "currency.save";

        public static implicit operator int(Currency currency) { return currency.money; }
    }

    public override string IdStr => "Progress";

    public Upgrades upgrades = new();
    public Currency currency = new();

    void OnEnable()
    {
        Load();
    }

    public void Save()
    {
        SaveSystem.Save(upgrades, Upgrades.FileName);
        SaveSystem.Save(currency, Currency.FileName);
    }

    public void Load()
    {
        upgrades = SaveSystem.Load<Upgrades>(Upgrades.FileName) ?? new();
        currency = SaveSystem.Load<Currency>(Currency.FileName) ?? new();
    }

    public void Clear()
    {
        SaveSystem.DeleteData(Upgrades.FileName);
        SaveSystem.DeleteData(Currency.FileName);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("[PROGRESS]/Find")]
    public static void Find()
    {
        FindObject();
    }

    [UnityEditor.MenuItem("[PROGRESS]/Save")]
    public static void SaveProgress()
    {
        Instance.Save();
    }

    [UnityEditor.MenuItem("[PROGRESS]/Load")]
    public static void LoadProgress()
    {
        Instance.Load();
    }

    [UnityEditor.MenuItem("[PROGRESS]/Clear")]
    public static void ClearProgress()
    {
        Instance.Clear();
    }
#endif
}