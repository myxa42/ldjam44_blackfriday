
using UnityEngine;

[CreateAssetMenu(menuName = "Specs/Inventory/Consumable")]
public sealed class InventoryItemConsumableSpec : InventoryItemSpec
{
    public Consumable.Visual Consumable;
    public int HealthBonusPercent;
}
