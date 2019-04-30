
using UnityEngine;

[CreateAssetMenu(menuName = "Specs/Inventory/Throwable")]
public sealed class InventoryItemThrowableSpec : InventoryItemSpec
{
    public Throwable.Visual Throwable;
    public int Damage;
    public int CritProbability;
}
