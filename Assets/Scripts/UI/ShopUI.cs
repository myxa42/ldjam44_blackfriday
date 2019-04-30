
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public sealed class ShopUI : MonoBehaviour
{
    struct ShopItem
    {
        public global::InventoryItem Item;
    }

    public AllInventoryItems AllInventoryItems;

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
    public GameObject ShopActionsPanel;
    public GameObject ShopActionsPanelContents;
    public GameObject BuyButton;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDamage;
    public TextMeshProUGUI ItemCrit;
    public TextMeshProUGUI ItemHealthBonus;
    public TextMeshProUGUI ItemSpecial;
    public PopupMessage BuyPopupMessage;

    [Header("Prefabs")]
    public GameObject LinePrefab;

    [Header("Localization")]
    public TextMeshProUGUI Hint;
    public TextMeshProUGUI Damage;
    public TextMeshProUGUI Crit;
    public TextMeshProUGUI Health;
    public TextMeshProUGUI BuyButtonText;
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
    private InventoryUI.Page mPage = InventoryUI.Page.Weapons;

    void Awake()
    {
        mUI = FindObjectOfType<UI>();

        mGameController = FindObjectOfType<GameController>();
        mGameController.Inventory.Changed += OnInventoryChanged;

        UpdateLanguage();
        UpdateInfoPanel();
        UpdateUI();

        ShopActionsPanel.SetActive(false);

        // Preallocate lines for the shop
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

    public void SetPage(InventoryUI.Page page)
    {
        if (mPage != page) {
            mPage = page;
            Refresh(true);
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        WeaponButtonNormal.SetActive(mPage != InventoryUI.Page.Weapons);
        WeaponButtonActive.SetActive(mPage == InventoryUI.Page.Weapons);
        ThrowButtonNormal.SetActive(mPage != InventoryUI.Page.Throwables);
        ThrowButtonActive.SetActive(mPage == InventoryUI.Page.Throwables);
        FoodButtonNormal.SetActive(mPage != InventoryUI.Page.Consumables);
        FoodButtonActive.SetActive(mPage == InventoryUI.Page.Consumables);
    }

    public void UpdateLanguage()
    {
        Damage.text = Language.Current.WeaponDamage;
        Crit.text = Language.Current.CriticalHitChance;
        Health.text = Language.Current.HealthBonus;
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
        if (item != null && item.Item != null && item.Item.Spec != null && !ShopActionsPanel.activeSelf) {
            mSelectedItem = item;
            UpdateInfoPanel();

            ShopActionsPanel.SetActive(true);
            ShopActionsPanelContents.GetComponent<RectTransform>().anchoredPosition = position;

            if (item.Item.Spec.ShopMoneyCost > 0) {
                string cost = $"<size=60%><color=#FFFF40>-{item.Item.Spec.ShopMoneyCost} coins";
                BuyButtonText.text = String.Format(Language.Current.BuyMoneyButton, cost);
            } else {
                string cost = $"<size=60%><color=#FF4040>-{item.Item.Spec.ShopHealthCost}";
                BuyButtonText.text = String.Format(Language.Current.BuyHealthButton, cost);
            }
        }
    }

    public void Refresh(bool clearSelection = true)
    {
        if (clearSelection && (mHoveredItem != null || mSelectedItem != null)) {
            mHoveredItem = null;
            mSelectedItem = null;
            UpdateInfoPanel();
        }

        var items = new List<ShopItem>();
        foreach (var it in AllInventoryItems.GetItems(mPage)) {
            if (it.ShopMoneyCost > 0 || it.ShopHealthCost > 0)
                items.Add(new ShopItem{ Item = new InventoryItem{ Spec = it } });
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
        if (mUI != null && mUI.CurrentState == UI.State.Shop)
            Refresh(false);
    }

    public void OnCloseMenuButtonClicked()
    {
        ShopActionsPanel.SetActive(false);
        mSelectedItem = null;
    }

    public void OnBuyButtonClicked()
    {
        if (mSelectedItem == null) {
            Debug.LogError("No selected item.");
            return;
        }

        var item = mSelectedItem.Item;

        if (item.Spec.ShopHealthCost > 0) {
            if (item.Spec.ShopHealthCost >= mGameController.Health) {
                BuyPopupMessage.ShowMessage(Language.Current.NotEnoughHealth, Color.red);
                return;
            }
            mGameController.AdjustHealth(-item.Spec.ShopHealthCost);
        } else {
            if (item.Spec.ShopMoneyCost > mGameController.Money) {
                BuyPopupMessage.ShowMessage(Language.Current.NotEnoughMoney, Color.red);
                return;
            }
            mGameController.AdjustMoney(-item.Spec.ShopMoneyCost);
        }

        mGameController.Inventory.AddItem(item.Spec);
        BuyPopupMessage.ShowMessage(Language.Current.ItemPurchased, Color.yellow);
    }
}
