
using System;
using System.Collections.Generic;

public sealed class InventoryItemThrowableSpecComparer : IComparer<InventoryItemThrowableSpec>
{
    public int Compare(InventoryItemThrowableSpec a, InventoryItemThrowableSpec b)
    {
        if (a.Damage > b.Damage)
            return -1;
        else if (a.Damage < b.Damage)
            return 1;

        if (a.CritProbability > b.CritProbability)
            return -1;
        else if (a.CritProbability < b.CritProbability)
            return 1;

        return String.Compare(a.GetLocalizedName(), b.GetLocalizedName(), true, Language.Current.CultureInfo);
    }
}
