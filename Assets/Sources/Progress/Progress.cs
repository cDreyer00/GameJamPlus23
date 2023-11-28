using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sources.cdreyer;
using Sources.cdreyer.SaveSystem;
using System;

[CreateAssetMenu(menuName = "Progress")]
public class Progress : SingletonSO<Progress>, ISavable
{
    public override string IdStr => "Progress";

    [Serializable]
    public class Upgrades
    {
        public int healthLevel;
        public int damageLevel;
        public int recoilLevel;
        public int brakingLevel;
        public int attackSpeedLevel;
    }

    [Serializable]
    public class Currency
    {
        public int money;
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