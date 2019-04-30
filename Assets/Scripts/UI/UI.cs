
using UnityEngine;
using TMPro;

public sealed class UI : MonoBehaviour
{
    public enum HudMode
    {
        Hidden,
        Passive,
        Active,
    }

    public enum State
    {
        Game,
        Inventory,
        Shop,
    }

    public const int HealthBarFillerWidth = 420;
    public const int ExperienceBarFillerWidth = 185;

    [Header("UI Elements")]
    public GameObject Hud;
    public GameObject ActionBar;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI ExperienceText;
    public TextMeshProUGUI MoneyText;
    public RectTransform HealthBarFiller;
    public RectTransform ExperienceBarFiller;
    public InventoryUI Inventory;
    public ShopUI Shop;
    public PopupMessage HealthPopupMessage;
    public PopupMessage ExperiencePopupMessage;
    public PopupMessage MoneyPopupMessage;
    public StatsPanelAnimator StatsPanelAnimator;
    public InventoryPanelAnimator InventoryPanelAnimator;
    public ShopPanelAnimator ShopPanelAnimator;
    public RectTransform HudStatsPanelContainer;
    public RectTransform InventoryStatsPanelContainer;
    public RectTransform ShopStatsPanelContainer;

    private GameController mGameController;
    private State mState = State.Game;
    private HudMode mHudMode = HudMode.Hidden;
    private long mLastKnownHealth;
    private long mLastKnownMaxHealth;
    private long mLastKnownExperience;
    private long mLastKnownNextExperience;
    private long mLastKnownMoney;
    private bool mFirstUpdate = true;

    public State CurrentState => mState;

    void Awake()
    {
        mGameController = FindObjectOfType<GameController>();
        UpdateUI();
    }

    public void SetState(State state)
    {
        if (mState != state) {
            mState = state;
            UpdateUI();
        }
    }

    public void SetHudMode(HudMode mode)
    {
        if (mHudMode != mode) {
            mHudMode = mode;
            UpdateUI();
        }
    }

    public void SetStateAndHudMode(State state, HudMode mode)
    {
        if (mState != state || mHudMode != mode) {
            mState = state;
            mHudMode = mode;
            UpdateUI();
        }
    }

    public static string FormatBigNumber(long value)
    {
        double dvalue;
        char suffix;

        if (value >= 1000000000000000L) {
            suffix = 'P';
            dvalue = (double)value / 1000000000000000.0;
        } else if (value >= 1000000000000L) {
            suffix = 'T';
            dvalue = (double)value / 1000000000000.0;
        } else if (value >= 1000000000L) {
            suffix = 'G';
            dvalue = (double)value / 1000000000.0;
        } else if (value >= 1000000L) {
            suffix = 'M';
            dvalue = (double)value / 1000000.0;
        } else if (value >= 1000L) {
            suffix = 'K';
            dvalue = (double)value / 1000.0;
        } else
            return $"{value}";

        return dvalue.ToString("F1", Language.Current.CultureInfo) + suffix;
    }

    void Update()
    {
        long health = mGameController.AnimatedHealth;
        long maxHealth = mGameController.MaxHealth;
        if (mLastKnownHealth != health || mLastKnownMaxHealth != maxHealth || mFirstUpdate) {
            mLastKnownHealth = health;
            mLastKnownMaxHealth = maxHealth;
            HealthText.text = $"{health} / {mGameController.MaxHealth}";

            float progress = (maxHealth > 0 ? (float)((double)health / maxHealth) : 0.0f);
            var off = HealthBarFiller.offsetMax;
            off.x = -Mathf.Clamp((1.0f - progress) * HealthBarFillerWidth, 0, HealthBarFillerWidth);
            HealthBarFiller.offsetMax = off;
        }

        long exp = mGameController.AnimatedExperience;
        long nextExp = mGameController.NextExperience;
        if (mLastKnownExperience != exp || mLastKnownNextExperience != nextExp || mFirstUpdate) {
            mLastKnownExperience = exp;
            mLastKnownNextExperience = nextExp;
            ExperienceText.text = $"{FormatBigNumber(exp)} / {FormatBigNumber(nextExp)}";

            float progress = (nextExp > 0 ? (float)((double)exp / nextExp) : 0.0f);
            var off = ExperienceBarFiller.offsetMax;
            off.x = -Mathf.Clamp((1.0f - progress) * ExperienceBarFillerWidth, 0, ExperienceBarFillerWidth);
            ExperienceBarFiller.offsetMax = off;
        }

        long money = mGameController.AnimatedMoney;
        if (mLastKnownMoney != money || mFirstUpdate) {
            mLastKnownMoney = money;
            MoneyText.text = FormatBigNumber(money);
        }

        mFirstUpdate = false;
    }

    public void UpdateUI()
    {
        Hud.SetActive(mHudMode != HudMode.Hidden);
        ActionBar.SetActive(mHudMode == HudMode.Active);
        Inventory.gameObject.SetActive(mState == State.Inventory || InventoryPanelAnimator.IsAnimating);
        Shop.gameObject.SetActive(mState == State.Shop || ShopPanelAnimator.IsAnimating);
    }

    public void OnSwitchEnemyButtonClicked()
    {
        if (mHudMode != HudMode.Active)
            return;

        mGameController.SwitchToNextEnemy();
    }

    public void OnMeleeAttackButtonClicked()
    {
        if (mHudMode != HudMode.Active)
            return;

        mGameController.PerformMeleeAttack();
    }

    public void OnUseItemButtonClicked()
    {
        if (mHudMode != HudMode.Active)
            return;

        SetState(State.Inventory);
        InventoryPanelAnimator.SetOpen(true);
        StatsPanelAnimator.SetTargetParent(InventoryStatsPanelContainer);
        Inventory.Refresh();
    }

    public void OpenShop()
    {
        SetState(State.Shop);
        ShopPanelAnimator.SetOpen(true);
        StatsPanelAnimator.SetTargetParent(ShopStatsPanelContainer);
        Shop.Refresh();
    }

    public void OnSettingsButtonClicked()
    {
        // FIXME
    }

    public void OnInventoryWeaponsButtonClicked()
    {
        Inventory.SetPage(InventoryUI.Page.Weapons);
    }

    public void OnInventoryThrowablesButtonClicked()
    {
        Inventory.SetPage(InventoryUI.Page.Throwables);
    }

    public void OnInventoryConsumablesButtonClicked()
    {
        Inventory.SetPage(InventoryUI.Page.Consumables);
    }

    public void OnCloseInventoryButtonClicked()
    {
        InventoryPanelAnimator.SetOpen(false);
        StatsPanelAnimator.SetTargetParent(HudStatsPanelContainer);
        SetState(State.Game);
    }

    public void OnShopWeaponsButtonClicked()
    {
        Shop.SetPage(InventoryUI.Page.Weapons);
    }

    public void OnShopThrowablesButtonClicked()
    {
        Shop.SetPage(InventoryUI.Page.Throwables);
    }

    public void OnShopConsumablesButtonClicked()
    {
        Shop.SetPage(InventoryUI.Page.Consumables);
    }

    public void OnCloseShopButtonClicked()
    {
        ShopPanelAnimator.SetOpen(false);
        StatsPanelAnimator.SetTargetParent(HudStatsPanelContainer);
        SetState(State.Game);
    }
}
