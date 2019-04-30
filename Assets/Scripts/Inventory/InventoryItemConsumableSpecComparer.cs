
using System;
using System.Collections.Generic;

public sealed class InventoryItemConsumableSpecComparer : IComparer<InventoryItemConsumableSpec>
{
    public int Compare(InventoryItemConsumableSpec a, InventoryItemConsumableSpec b)
    {
        if (a.HealthBonusPercent > b.HealthBonusPercent)
            return -1;
        else if (a.HealthBonusPercent < b.HealthBonusPercent)
            return 1;

        return String.Compare(a.GetLocalizedName(), b.GetLocalizedName(), true, Language.Current.CultureInfo);
    }
}
