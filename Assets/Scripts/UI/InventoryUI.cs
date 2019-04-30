
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public sealed class InventoryUI : MonoBehaviour
{
    public enum Page
    {
        Weapons,
        Throwables,
        Consumables,
    }

    struct InventoryItem
    {
        public global::InventoryItem Item;
    }

    [Header("UI Elements")]
    public GameObject Content;
    public GameObject WeaponButtonNormal;
    public GameObject WeaponButtonActive;
    public GameObject ThrowButtonNormal;
    public GameObject ThrowButtonActive;
    public GameObject FoodButtonNormal;
    public GameObject FoodButtonActive;
    public GameObject InfoPanelContents;
    public GameObject InfoPanelSkulls;
    public GameObject[] InfoPanelSkullActiveIcons;
    public GameObject[] InfoPanelSkullInactiveIcons;
    public GameObject SpecialIconContainer;
    public GameObject PoisonIcon;
    public GameObject ConfuseIcon;
    public GameObject StunIcon;
    public GameObject ScareIcon;
    public GameObject InfuriateIcon;
    public GameObject InventoryActionsPanel;
    public GameObject InventoryActionsPanelContents;
    public GameObject NoActions;
    public GameObject EquipButton;
    public GameObject ThrowButton;
    public GameObject EatButton;
    public GameObject UpgradeButton;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDamage;
    public TextMeshProUGUI ItemCrit;
    public TextMeshProUGUI ItemHealthBonus;
    public TextMeshProUGUI ItemSpecial;
    public PopupMessage UpgradePopupMessage;

    [Header("Prefabs")]
    public GameObject LinePrefab;

    [Header("Localization")]
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Hint;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI Crit;
    public TextMeshProUGUI Health;
    public TextMeshProUGUI NoActionsText;
    public TextMeshProUGUI EquipButtonText;
    public TextMeshProUGUI ThrowButtonText;
    public TextMeshProUGUI EatButtonText;
    public TextMeshProUGUI UpgradeButtonText;
    public TextMeshProUGUI CancelButtonText;

    [Header("Style")]
    public Image PoisonIconColor;
    public Image ConfuseIconColor;
    public Image StunIconColor;
    public Image ScareIconColor;
    public Image InfuriateIconColor;

    private UI mUI;
    private GameController mGameController;
    private InventoryItemUI mHoveredItem;
    private InventoryItemUI mSelectedItem;
    private Page mPage = Page.Weapons;

    void Awake()
    {
        mUI = FindObjectOfType<UI>();

        mGameController = FindObjectOfType<GameController>();
        mGameController.Inventory.Changed += OnInventoryChanged;

        UpdateLanguage();
        UpdateInfoPanel();
        UpdateUI();

        InventoryActionsPanel.SetActive(false);

        // Preallocate lines for the inventory
        for (int i = 0; i < 50; i++) {
            var line = Instantiate(LinePrefab, Content.transform);
            line.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (mGameController != null && mGameController.Inventory != null)
            mGameController.Inventory.Changed -= OnInventoryChanged;
    }

    public void SetPage(Page page)
    {
        if (mPage != page) {
            mPage = page;
            Refresh(true);
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        WeaponButtonNormal.SetActive(mPage != Page.Weapons);
        WeaponButtonActive.SetActive(mPage == Page.Weapons);
        ThrowButtonNormal.SetActive(mPage != Page.Throwables);
        ThrowButtonActive.SetActive(mPage == Page.Throwables);
        FoodButtonNormal.SetActive(mPage != Page.Consumables);
        FoodButtonActive.SetActive(mPage == Page.Consumables);
    }

    public void UpdateLanguage()
    {
        Title.text = Language.Current.Inventory;
        Damage.text = Language.Current.WeaponDamage;
        Crit.text = Language.Current.CriticalHitChance;
        Health.text = Language.Current.HealthBonus;
        NoActionsText.text = Language.Current.NoActions;
        EquipButtonText.text = Language.Current.EquipButton;
        ThrowButtonText.text = Language.Current.ThrowButton;
        EatButtonText.text = Language.Current.EatButton;
        CancelButtonText.text = Language.Current.CancelButton;
        switch (SystemInfo.deviceType) {
            case DeviceType.Handheld: Hint.text = Language.Current.TapHint; break;
            case DeviceType.Console: Hint.text = Language.Current.SelectHint; break;
            case DeviceType.Desktop: Hint.text = Language.Current.HoverHint; break;
        }
    }

    public void HoverItem(InventoryItemUI item, bool hover)
    {
        if (hover) {
            if (mHoveredItem != item) {
                mHoveredItem = item;
                UpdateInfoPanel();
            }
        } else {
            if (mHoveredItem == item) {
                mHoveredItem = null;
                UpdateInfoPanel();
            }
        }
    }

    public void ClickItem(InventoryItemUI item, Vector2 position)
    {
        if (item != null && item.Item != null && item.Item.Spec != null && !InventoryActionsPanel.activeSelf) {
            mSelectedItem = item;
            UpdateInfoPanel();

            InventoryActionsPanel.SetActive(true);
            InventoryActionsPanelContents.GetComponent<RectTransform>().anchoredPosition = position;

            var weapon = mSelectedItem.Item.Spec as InventoryItemWeaponSpec;

            bool isWeapon = weapon != null;
            bool isFood = mSelectedItem.Item.Spec is InventoryItemConsumableSpec;
            bool isThrowable = mSelectedItem.Item.Spec is InventoryItemThrowableSpec;
            bool isEquipped = mSelectedItem.Item == mGameController.EquippedWeapon;

            bool canThrow = !isEquipped && isThrowable;
            bool canEat = isFood;
            bool canUpgrade = isWeapon && weapon.CanUpgrade();
            bool canEquip = isWeapon && !isEquipped;

            if (canUpgrade) {
                string cost = $"<size=60%><color=#FF4040>-{weapon.GetUpgradeCost()}";
                UpgradeButtonText.text = String.Format(Language.Current.UpgradeButton, cost);
            }

            NoActions.SetActive(!canThrow && !canEat && !canUpgrade && !canEquip);
            ThrowButton.SetActive(canThrow);
            EatButton.SetActive(canEat);
            UpgradeButton.SetActive(canUpgrade);
            EquipButton.SetActive(canEquip);
        }
    }

    public void Refresh(bool clearSelection = true)
    {
        if (clearSelection && (mHoveredItem != null || mSelectedItem != null)) {
            mHoveredItem = null;
            mSelectedItem = null;
            UpdateInfoPanel();
        }

        var items = new List<InventoryItem>();
        foreach (var list in mGameController.Inventory.GetItems(mPage)) {
            foreach (var it in list)
                items.Add(new InventoryItem{ Item = it });
        }

        int index = 0;
        int itemsN = items.Count;

        var t = Content.transform;
        int linesN = t.childCount;

        for (int i = 0; i < linesN || index < itemsN; i++) {
            GameObject lineGO;
            if (i < linesN)
                lineGO = t.GetChild(i).gameObject;
            else
                lineGO = Instantiate(LinePrefab, Content.transform);

            if (index >= itemsN) {
                lineGO.SetActive(false);
                continue;
            }

            lineGO.SetActive(true);

            var line = lineGO.GetComponent<InventoryLineUI>();
            var lineItems = line.Items;
            int lineItemsN = lineItems.Length;

            for (int j = 0; j < lineItemsN; j++) {
                var itemUI = lineItems[j];
                if (index >= itemsN) {
                    itemUI.Contents.SetActive(false);
                    itemUI.Item = null;
                } else {
                    var item = items[index];
                    var weapon = item.Item.Spec as InventoryItemWeaponSpec;

                    itemUI.Contents.SetActive(true);
                    itemUI.Icon.sprite = item.Item.Spec.Icon;
                    itemUI.Item = item.Item;
                    itemUI.SetSkullCount(weapon != null ? weapon.Level : 0);
                    itemUI.SetIsEquipped(mGameController.EquippedWeapon == item.Item);
                }
                ++index;
            }
        }
    }

    void UpdateInfoPanel()
    {
        var item = mSelectedItem;
        if (item == null)
            item = mHoveredItem;

        if (item == null || item.Item == null || item.Item.Spec == null) {
            Hint.gameObject.SetActive(true);
            InfoPanelContents.SetActive(false);
        } else {
            Hint.gameObject.SetActive(false);
            InfoPanelContents.SetActive(true);

            ItemName.text = item.Item.Spec.GetLocalizedName();
            switch (item.Item.Spec) {
                case InventoryItemWeaponSpec weapon:
                    InfoPanelSkulls.SetActive(true);
                    Damage.gameObject.SetActive(true);
                    Crit.gameObject.SetActive(true);
                    Health.gameObject.SetActive(false);
                    ItemDamage.text = $"{weapon.BaseDamage}";
                    ItemCrit.text = $"{weapon.BaseCritProbability}%";
                    for (int i = 0; i < InfoPanelSkullActiveIcons.Length; i++) {
                        bool active = i < weapon.Level;
                        InfoPanelSkullActiveIcons[i].SetActive(active);
                        InfoPanelSkullInactiveIcons[i].SetActive(!active);
                    }
                    if (weapon.Special == Weapon.Special.None)
                        SpecialIconContainer.SetActive(false);
                    else {
                        SpecialIconContainer.SetActive(true);
                        PoisonIcon.SetActive(weapon.Special == Weapon.Special.Poison);
                        ConfuseIcon.SetActive(weapon.Special == Weapon.Special.Confuse);
                        StunIcon.SetActive(weapon.Special == Weapon.Special.Stun);
                        ScareIcon.SetActive(weapon.Special == Weapon.Special.Scare);
                        InfuriateIcon.SetActive(weapon.Special == Weapon.Special.Infuriate);
                        ItemSpecial.text = $"{weapon.SpecialProbability}%";
                        switch (weapon.Special) {
                            case Weapon.Special.Poison: ItemSpecial.color = PoisonIconColor.color; break;
                            case Weapon.Special.Confuse: ItemSpecial.color = ConfuseIconColor.color; break;
                            case Weapon.Special.Stun: ItemSpecial.color = StunIconColor.color; break;
                            case Weapon.Special.Scare: ItemSpecial.color = ScareIconColor.color; break;
                            case Weapon.Special.Infuriate: ItemSpecial.color = InfuriateIconColor.color; break;
                        }
                    }
                    break;

                case InventoryItemThrowableSpec throwable:
                    InfoPanelSkulls.SetActive(false);
                    SpecialIconContainer.SetActive(false);
                    Damage.gameObject.SetActive(true);
                    Crit.gameObject.SetActive(true);
                    Health.gameObject.SetActive(false);
                    ItemDamage.text = $"{throwable.Damage}";
                    ItemCrit.text = $"{throwable.CritProbability}%";
                    break;

                case InventoryItemConsumableSpec consumable:
                    InfoPanelSkulls.SetActive(false);
                    SpecialIconContainer.SetActive(false);
                    Damage.gameObject.SetActive(false);
                    Crit.gameObject.SetActive(false);
                    Health.gameObject.SetActive(true);
                    ItemHealthBonus.text = $"+{consumable.HealthBonusPercent}%";
                    break;

                default:
                    InfoPanelSkulls.SetActive(false);
                    SpecialIconContainer.SetActive(false);
                    Damage.gameObject.SetActive(false);
                    Crit.gameObject.SetActive(false);
                    Health.gameObject.SetActive(false);
                    break;
            }
        }
    }

    void OnInventoryChanged()
    {
        if (mUI != null && mUI.CurrentState == UI.State.Inventory)
            Refresh(false);
    }

    public void OnCloseMenuButtonClicked()
    {
        InventoryActionsPanel.SetActive(false);
        mSelectedItem = null;
    }

    public void OnEquipButtonClicked()
    {
        if (mSelectedItem == null) {
            Debug.LogError("No selected item.");
            return;
        }

        var item = mSelectedItem.Item;
        var spec = item.Spec as InventoryItemWeaponSpec;
        if (spec == null)
            return;

        if (mGameController.EquippedWeapon != item) {
            mGameController.EquipWeapon(item);
            Refresh(false);
        }

        InventoryActionsPanel.SetActive(false);
        mSelectedItem = null;
        UpdateInfoPanel();

        mUI.OnCloseInventoryButtonClicked();
    }

    public void OnThrowButtonClicked()
    {
        if (mSelectedItem == null) {
            Debug.LogError("No selected item.");
            return;
        }

        var item = mSelectedItem.Item;
        var spec = item.Spec as InventoryItemThrowableSpec;
        if (spec == null)
            return;

        InventoryActionsPanel.SetActive(false);
        mSelectedItem = null;
        UpdateInfoPanel();

        if (!mGameController.Inventory.RemoveItem(item, notifyChanged: false))
            return;

        mUI.OnCloseInventoryButtonClicked();
        mGameController.PerformThrowAttack(spec.Throwable);
    }

    public void OnUpgradeButtonClicked()
    {
        if (mSelectedItem == null) {
            Debug.LogError("No selected item.");
            return;
        }

        var item = mSelectedItem.Item;
        var spec = item.Spec as InventoryItemWeaponSpec;
        if (spec == null || !spec.CanUpgrade())
            return;

        var upgradeCost = spec.GetUpgradeCost();
        if (upgradeCost >= mGameController.Health) {
            UpgradePopupMessage.ShowMessage(Language.Current.NotEnoughHealth, Color.red);
            return;
        }

        InventoryActionsPanel.SetActive(false);
        mSelectedItem = null;
        UpdateInfoPanel();

        bool wasEquipped = mGameController.EquippedWeapon == item;
        if (wasEquipped)
            mGameController.EquipWeapon(null);

        if (!mGameController.Inventory.RemoveItem(item, notifyChanged: false))
            return;

        var upgradedSpec = spec.WithLevel(spec.Level + 1);
        item = mGameController.Inventory.AddItem(upgradedSpec, notifyChanged: false);

        mGameController.AdjustHealth(-upgradeCost);

        if (wasEquipped)
            mGameController.EquipWeapon(item);

        UpgradePopupMessage.ShowMessage(Language.Current.WeaponUpgraded, Color.yellow);
        mGameController.Inventory.NotifyChanged();
    }

    public void OnEatButtonClicked()
    {
        if (mSelectedItem == null) {
            Debug.LogError("No selected item.");
            return;
        }

        var item = mSelectedItem.Item;
        var spec = item.Spec as InventoryItemConsumableSpec;
        if (spec == null)
            return;

        InventoryActionsPanel.SetActive(false);
        mSelectedItem = null;
        UpdateInfoPanel();

        if (!mGameController.Inventory.RemoveItem(item))
            return;

        mGameController.AdjustHealth(spec.HealthBonusPercent * mGameController.MaxHealth / 100L);
    }
}
