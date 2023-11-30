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
    public override string IdStr => "Progress";

    [Serializable]
    public class Upgrades
    {
        public enum Type { Health, Damage, Recoil, Barking, AttackSpeed }
        public ClampedPrimitive<int> healthLevel;
        public ClampedPrimitive<int> damageLevel;
        public ClampedPrimitive<int> recoilLevel;
        public ClampedPrimitive<int> brakingLevel;
        public ClampedPrimitive<int> attackSpeedLevel;

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
    }

    [Serializable]
    public class Currency
    {
        public int money;

        public static implicit operator int(Currency currency) { return currency.money; }
    }

    public Upgrades upgrades;
    public Currency currency;

    public void Save()
    {
        SaveSystem.Save(upgrades, "upgrades.save");
        SaveSystem.Save(currency, "currency.save");
    }

    public void Load()
    {
        upgrades = SaveSystem.Load<Upgrades>("upgrades.save");
        currency = SaveSystem.Load<Currency>("currency.save");
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
#endif
}