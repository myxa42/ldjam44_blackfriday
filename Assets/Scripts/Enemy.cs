
using UnityEngine;

public sealed class Enemy : MonoBehaviour
{
    const float HealthBarFillerWidth = 302.0f;

    public WeaponGroups WeaponGroups;
    public RectTransform HealthBarFiller;
    public int HealthAnimateSpeed = 2;
    public int AppearSpeed = 5;
    public int DeathSinkSpeed = 4;

    Character mCharacter;
    GameController mGameController;
    InventoryItemWeaponSpec mWeapon;
    long mVisibleHealth;
    long mTargetHealth;
    long mAnimatedHealth;
    int mDead;
    bool mAppearing;

    public bool IsAppearing => mAppearing;
    public Character Character => mCharacter;
    public long Health => mTargetHealth;
    public long AnimatedHealth => mAnimatedHealth;
    public long MaxHealth { get; private set; }

    void Awake()
    {
        mGameController = FindObjectOfType<GameController>();
        mVisibleHealth = -1;
    }

    void Start()
    {
        Randomize();
    }

    public bool IsPlayingDeathAnimation()
    {
        return (mDead > 0 && mDead < 3);
    }

    void Update()
    {
        if (mAppearing) {
            var t = transform;
            Vector3 position = t.position;
            position.x -= AppearSpeed * Time.deltaTime;
            t.position = position;

            if (position.x <= Character.OriginalPosition.x) {
                position.x = Character.OriginalPosition.x;
                mCharacter.SetFlipHorizontally(false);
                mCharacter.SetAnimation(Character.Animation.Idle, 0.2f);
                mAppearing = false;
            }
        }

        if (mDead != 0) {
            if (mDead == 1 && !Character.IsPlayingAnimation())
                mDead = 2;

            if (mDead == 2) {
                var t = transform;
                Vector3 position = t.position;
                position.y -= DeathSinkSpeed * Time.deltaTime;
                t.position = position;

                if (Character.OriginalPosition.y - position.y > 1.0f)
                    mDead = 3;
            }

            return;
        }

        Util.Animate(ref mAnimatedHealth, mTargetHealth, HealthAnimateSpeed);

        if (mVisibleHealth != mAnimatedHealth) {
            mVisibleHealth = mAnimatedHealth;
            float progress = (MaxHealth > 0 ? (float)((double)AnimatedHealth / MaxHealth) : 0.0f);
            var off = HealthBarFiller.offsetMax;
            off.x = -Mathf.Clamp((1.0f - progress) * HealthBarFillerWidth, 0, HealthBarFillerWidth);
            HealthBarFiller.offsetMax = off;
        }

        if (mTargetHealth <= 0) {
            Character.LifeBarContainer.SetActive(false);
            Character.SetAnimation(Character.Animation.Death);
            mGameController.AdjustMoney(10); // FIXME
            mDead = 1;
        }
    }

    public void ApplyDamage(long value)
    {
        if (value < 0) {
            Debug.LogError("Negative damage!!!");
            return;
        }

        mTargetHealth -= value;
        if (mTargetHealth > MaxHealth)
            mTargetHealth = MaxHealth;
        if (mTargetHealth < 0)
            mTargetHealth = 0;

        var message = $"-{UI.FormatBigNumber(value)}";
        Character.PopupMessage.ShowMessage(message, new Color32(231, 39, 45, 255));
    }

    public void Randomize()
    {
        mCharacter = GetComponent<Character>();
        mCharacter.SetAnimation(Character.Animation.Run);
        mCharacter.SetRandomVisual();
        mCharacter.LifeBarContainer.SetActive(true);

        transform.position = mCharacter.OriginalPosition + new Vector3(7.0f, 0.0f, 0.0f);
        mCharacter.RootVisual.transform.eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
        mAppearing = true;

        // FIXME
        mWeapon = WeaponGroups.SelectRandom(WeaponGroups.Group1);
        mCharacter.SetWeapon(mWeapon.Weapon);

        long health = Random.Range(25, 40 + 1) * 2;// * mGameController.MaxHealth / 100;
        Debug.Log($"Enemy health: {health}");
        MaxHealth = health;
        mAnimatedHealth = health;
        mTargetHealth = health;
        mVisibleHealth = -1;
        mDead = 0;
    }

    public void SetArrowVisible(bool flag)
    {
        mCharacter.SetArrowVisible(flag);
    }
}
