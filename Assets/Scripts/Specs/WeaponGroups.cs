
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Specs/Weapon Groups")]
public sealed class WeaponGroups : ScriptableObject
{
    public List<InventoryItemWeaponSpec> Group1;
    public List<InventoryItemWeaponSpec> Group2;
    public List<InventoryItemWeaponSpec> Group3;

    public static InventoryItemWeaponSpec SelectRandom(List<InventoryItemWeaponSpec> list)
    {
        int index = Random.Range(0, list.Count);
        return list[index];
    }

    public static InventoryItemWeaponSpec SelectRandom(List<InventoryItemWeaponSpec> list1,
        List<InventoryItemWeaponSpec> list2)
    {
        int index = Random.Range(0, list1.Count + list2.Count);
        return (index < list1.Count ? list1[index] : list2[index - list1.Count]);
    }

    public static InventoryItemWeaponSpec SelectRandom(List<InventoryItemWeaponSpec> list1,
        List<InventoryItemWeaponSpec> list2, List<InventoryItemWeaponSpec> list3)
    {
        int index = Random.Range(0, list1.Count + list2.Count + list3.Count);
        if (index < list1.Count)
            return list1[index];

        index -= list1.Count;
        if (index < list2.Count)
            return list2[index];

        return list3[index - list2.Count];
    }

    static InventoryItemWeaponSpec FindWeapon(List<InventoryItemWeaponSpec> list, Weapon.Visual visual)
    {
        foreach (var it in list) {
            if (it.Weapon == visual)
                return it;
        }

        return null;
    }

    public InventoryItemWeaponSpec WeaponFromEnum(Weapon.Visual visual)
    {
        var it = FindWeapon(Group1, visual);
        if (it == null)
            it = FindWeapon(Group2, visual);
        if (it == null)
            it = FindWeapon(Group3, visual);

        if (it == null)
            Debug.LogError($"Weapon {visual} is not in WeaponGroups!!!");

        return it;
    }
}
