
using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class Inventory
{
    private readonly SortedDictionary<InventoryItemWeaponSpec, List<InventoryItem>> mWeapons =
        new SortedDictionary<InventoryItemWeaponSpec, List<InventoryItem>>(new InventoryItemWeaponSpecComparer());
    private readonly SortedDictionary<InventoryItemThrowableSpec, List<InventoryItem>> mThrowables =
        new SortedDictionary<InventoryItemThrowableSpec, List<InventoryItem>>(new InventoryItemThrowableSpecComparer());
    private readonly SortedDictionary<InventoryItemConsumableSpec, List<InventoryItem>> mConsumables =
        new SortedDictionary<InventoryItemConsumableSpec, List<InventoryItem>>(new InventoryItemConsumableSpecComparer());

    public event Action Changed;

    static List<InventoryItem> GetList<T>(SortedDictionary<T, List<InventoryItem>> dict, T key, bool create)
    {
        if (!dict.TryGetValue(key, out List<InventoryItem> list)) {
            if (!create)
                return null;
            list = new List<InventoryItem>();
            dict[key] = list;
        }
        return list;
    }

    List<InventoryItem> GetListForSpec(InventoryItemSpec key, bool create)
    {
        if (key is InventoryItemWeaponSpec)
            return GetList(mWeapons, (InventoryItemWeaponSpec)key, create);
        else if (key is InventoryItemThrowableSpec)
            return GetList(mThrowables, (InventoryItemThrowableSpec)key, create);
        else if (key is InventoryItemConsumableSpec)
            return GetList(mConsumables, (InventoryItemConsumableSpec)key, create);
        else {
            string message = $"No inventory for \"{key.GetType().FullName}\".";
            if (!create) {
                Debug.LogError(message);
                return null;
            }
            throw new Exception(message);
        }
    }

    public InventoryItem AddItem(InventoryItemSpec spec, bool notifyChanged = true)
    {
        var item = new InventoryItem{ Spec = spec };

        List<InventoryItem> list = GetListForSpec(spec, true);
        list.Add(item);

        if (notifyChanged)
            Changed?.Invoke();

        return item;
    }

    public bool RemoveItem(InventoryItem item, bool notifyChanged = true)
    {
        List<InventoryItem> list = GetListForSpec(item.Spec, true);
        if (list.Remove(item)) {
            if (notifyChanged)
                Changed?.Invoke();
            return true;
        }

        Debug.LogError("Attempted to remove non-existent item from the inventory.");
        return false;
    }

    public IEnumerable<IList<InventoryItem>> GetItems(InventoryUI.Page page)
    {
        switch (page) {
            case InventoryUI.Page.Weapons:
                foreach (var it in mWeapons)
                    yield return it.Value;
                break;

            case InventoryUI.Page.Throwables:
                foreach (var it in mThrowables)
                    yield return it.Value;
                break;

            case InventoryUI.Page.Consumables:
                foreach (var it in mConsumables)
                    yield return it.Value;
                break;
        }
    }

    public void NotifyChanged()
    {
        Changed?.Invoke();
    }
}
