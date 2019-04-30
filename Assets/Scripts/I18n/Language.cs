
using UnityEngine;
using System;
using System.Globalization;

// WARNING! Be careful when changing this enum,
//          as it is being used in the settings file
public enum LanguageId
{
    SystemDefault = 0,
    English = 1,
    Spanish = 2,
    Russian = 3,
}

public interface ILanguage
{
    LanguageId Id { get; }
    CultureInfo CultureInfo { get; }

    string Black { get; }
    string Friday { get; }
    string Shopping { get; }
    string Loading { get; }

    string Inventory { get; }

    string HoverHint { get; }
    string SelectHint { get; }
    string TapHint { get; }

    string WeaponDamage { get; }
    string CriticalHitChance { get; }
    string HealthBonus { get; }

    string NoActions { get; }
    string EquipButton { get; }
    string ThrowButton { get; }
    string EatButton { get; }
    string UpgradeButton { get; }
    string BuyHealthButton { get; }
    string BuyMoneyButton { get; }
    string CancelButton { get; }

    string LevelUp { get; }

    string WeaponUpgraded { get; }
    string NotEnoughHealth { get; }

    string ItemPurchased { get; }
    string NotEnoughMoney { get; }
}

public abstract class Language
{
    public static ILanguage Current { get; private set; }
    public static LanguageId CurrentId => (CurrentIsSystemDefault ? LanguageId.SystemDefault : Current.Id);
    public static bool CurrentIsSystemDefault { get; private set; }
    public static readonly ILanguage SystemDefault;

    static Language()
    {
        // FIXME
        SystemDefault = new EnglishLanguage();

        Current = SystemDefault;
        CurrentIsSystemDefault = true;
    }

    public static void SetLanguage(LanguageId id)
    {
        switch (id) {
            case LanguageId.SystemDefault:
                Current = SystemDefault;
                CurrentIsSystemDefault = true;
                break;

            case LanguageId.English:
                Current = new EnglishLanguage();
                CurrentIsSystemDefault = false;
                break;

            case LanguageId.Spanish:
                // FIXME
                Current = new EnglishLanguage();
                CurrentIsSystemDefault = false;
                break;

            case LanguageId.Russian:
                // FIXME
                Current = new EnglishLanguage();
                CurrentIsSystemDefault = false;
                break;

            default:
                Debug.LogError("Invalid language id \"{id}\".");
                break;
        }
    }
}
