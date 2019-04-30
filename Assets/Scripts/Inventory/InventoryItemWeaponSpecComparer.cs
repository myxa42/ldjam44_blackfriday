
using System;
using System.Collections.Generic;

public sealed class InventoryItemWeaponSpecComparer : IComparer<InventoryItemWeaponSpec>
{
    static int GetSpecialUsefullness(Weapon.Special s)
    {
        switch (s) {
            case Weapon.Special.Infuriate: return -1;
            case Weapon.Special.None: return 0;
            case Weapon.Special.Scare: return 1;
            case Weapon.Special.Poison: return 2;
            case Weapon.Special.Stun: return 2;
            case Weapon.Special.Confuse: return 2;
        }

        return 0;
    }

    public int Compare(InventoryItemWeaponSpec a, InventoryItemWeaponSpec b)
    {
        if (a.BaseDamage > b.BaseDamage)
            return -1;
        else if (a.BaseDamage < b.BaseDamage)
            return 1;

        int specialA = GetSpecialUsefullness(a.Special);
        int specialB = GetSpecialUsefullness(b.Special);
        if (specialA > specialB)
            return -1;
        else if (specialA < specialB)
            return 1;

        if (a.BaseCritProbability > b.BaseCritProbability)
            return -1;
        else if (a.BaseCritProbability < b.BaseCritProbability)
            return 1;

        if (a.Level > b.Level)
            return -1;
        else if (a.Level < b.Level)
            return 1;

        return String.Compare(a.GetLocalizedName(), b.GetLocalizedName(), true, Language.Current.CultureInfo);
    }
}
