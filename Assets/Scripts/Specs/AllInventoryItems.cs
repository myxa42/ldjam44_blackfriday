
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Specs/All Inventory Items")]
public sealed class AllInventoryItems : ScriptableObject
{
    public List<InventoryItemWeaponSpec> Weapons;
    public List<InventoryItemConsumableSpec> Consumables;
    public List<InventoryItemThrowableSpec> Throwables;

    public IEnumerable<InventoryItemSpec> GetItems(InventoryUI.Page page)
    {
        switch (page) {
            case InventoryUI.Page.Weapons:
                foreach (var it in Weapons)
                    yield return it;
                break;

            case InventoryUI.Page.Throwables:
                foreach (var it in Throwables)
                    yield return it;
                break;

            case InventoryUI.Page.Consumables:
                foreach (var it in Consumables)
                    yield return it;
                break;
        }
    }
}
