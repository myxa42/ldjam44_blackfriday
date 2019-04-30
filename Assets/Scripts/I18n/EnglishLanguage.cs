
using System.Globalization;

public sealed class EnglishLanguage : Language, ILanguage
{
    public LanguageId Id => LanguageId.English;
    public CultureInfo CultureInfo { get; } = new CultureInfo("en-GB");

    public string Black => "BLACK";
    public string Friday => "FRIDAY";
    public string Shopping => "SHOPPING";
    public string Loading => "UP TO {0}% FOR LIMITED TIME ONLY";

    public string Inventory => "Inventory";

    public string HoverHint => "Hover cursor over an item\nto see more details";
    public string SelectHint => "Select an item to see more details";
    public string TapHint => "Tap on an item to see more details";

    public string WeaponDamage => "Damage:";
    public string CriticalHitChance => "Critical hit chance:";
    public string HealthBonus => "Health:";

    public string NoActions => "No actions";
    public string EquipButton => "Equip";
    public string ThrowButton => "Throw";
    public string EatButton => "Eat";
    public string UpgradeButton => "Upgrade\n{0} health";
    public string BuyHealthButton => "Buy\n{0} health";
    public string BuyMoneyButton => "Buy\n{0}";
    public string CancelButton => "Cancel";

    public string LevelUp => "LEVEL UP!";

    public string WeaponUpgraded => "Weapon upgraded!";
    public string NotEnoughHealth => "Not enough health points!";

    public string ItemPurchased => "Item purchased!";
    public string NotEnoughMoney => "Not enough money!";
}
