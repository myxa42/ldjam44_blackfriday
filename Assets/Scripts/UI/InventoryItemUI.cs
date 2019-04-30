
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class InventoryItemUI : MonoBehaviour,
    IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Contents;
    public InventoryItem Item;
    public Image Icon;
    public Image Slot;
    public GameObject Skull1;
    public GameObject Skull2;
    public GameObject Skull3;

    private UI mUI;
    private InventoryUI mInventoryUI;
    private ShopUI mShopUI;

    void Awake()
    {
        mUI = FindObjectOfType<UI>();
        mInventoryUI = FindObjectOfType<InventoryUI>();
        mShopUI = FindObjectOfType<ShopUI>();
    }

    void OnDestroy()
    {
        if (mInventoryUI != null)
            mInventoryUI.HoverItem(this, false);
        if (mShopUI != null)
            mShopUI.HoverItem(this, false);
    }

    public void SetSkullCount(int count)
    {
        Skull1.SetActive(count > 0);
        Skull2.SetActive(count > 1);
        Skull3.SetActive(count > 2);
    }

    public void SetIsEquipped(bool flag)
    {
        if (!flag)
            Slot.color = Color.white;
        else
            Slot.color = new Color32(141, 198, 63, 255);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mUI.CurrentState == UI.State.Inventory && mInventoryUI != null)
            mInventoryUI.HoverItem(this, true);
        if (mUI.CurrentState == UI.State.Shop && mShopUI != null)
            mShopUI.HoverItem(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mUI.CurrentState == UI.State.Inventory && mInventoryUI != null)
            mInventoryUI.HoverItem(this, false);
        if (mUI.CurrentState == UI.State.Shop && mShopUI != null)
            mShopUI.HoverItem(this, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != 0)
            return;

        RectTransform transform;
        if (mUI.CurrentState == UI.State.Inventory)
            transform = mInventoryUI.InventoryActionsPanel.GetComponent<RectTransform>();
        else if (mUI.CurrentState == UI.State.Shop)
            transform = mShopUI.ShopActionsPanel.GetComponent<RectTransform>();
        else
            return;

        Vector2 position;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform,
                eventData.position, eventData.pressEventCamera, out position))
            return;

        if (mUI.CurrentState == UI.State.Inventory)
            mInventoryUI.ClickItem(this, position);
        else if (mUI.CurrentState == UI.State.Shop)
            mShopUI.ClickItem(this, position);
    }
}
