
using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class GameController : MonoBehaviour
{
    public enum State
    {
        Loading,
        EnterPOI,
        RunTowardsPOI,
        RunTowardsEndOfLevel,
        WaitShopClosed,
        BeginFight,
        WaitAppear,
        PlayerInput,
        EnemyMove,
        MeleeRunToEnemy,
        MeleeAttack,
        MeleeRunBack,
        RangedAttackCharge,
        RangedAttack,
        ThrowAttack,
        AttackEnded,
        CheckWinCondition,
        Ended,
    }

    public const long MinGainedExperience = 25;
    public const long MaxGainedExperience = 40;
    public const long FirstExperienceThreshold = 100;

    public Throwable ThrowablePrefab;

    private UI mUI;
    private Player mPlayer;
    private Level mLevel;
    private Character[] mAllCharacters;
    private Enemy[] mAllEnemies;
    private List<Character> mCharacters;
    private List<Enemy> mEnemies;
    private Character mTargetCharacter;
    private int mCurrentCharacter;
    private int mSelectedEnemy;
    private long mAnimatedPlayerHealth;
    private long mAnimatedPlayerExperience;
    private long mAnimatedPlayerMoney;
    private float mRangedAttackChargeTime;
    private float mMeleeAttackTime;
    private bool mMeleeDidDamage;
    private float mRangedAttackTime;
    private bool mRangedDidDamage;
    private float mThrowAttackTime;
    private Throwable.Visual mThrownItem;
    private int mCurrentPOI = 0;
    private State mState = State.Loading;

    public InventoryItem EquippedWeapon => Global.Instance.EquippedWeapon;
    public int PlayerLevel => Global.Instance.PlayerLevel;
    public long Health => Global.Instance.PlayerTargetHealth;
    public long MaxHealth => Global.Instance.MaxHealth;
    public long AnimatedHealth => mAnimatedPlayerHealth;
    public long Experience => Global.Instance.PlayerTargetExperience;
    public long AnimatedExperience => mAnimatedPlayerExperience;
    public long Money => Global.Instance.PlayerTargetMoney;
    public long AnimatedMoney => mAnimatedPlayerMoney;
    public Inventory Inventory => Global.Instance.PlayerInventory;

    public long NextExperience => (Global.Instance.LevelBaseExperience != 0
        ? Global.Instance.LevelBaseExperience + Global.Instance.LevelBaseExperience / 2
        : FirstExperienceThreshold);

    public long HealthAnimateSpeed = 5;
    public long ExperienceAnimateSpeed = 5;
    public long MoneyAnimateSpeed = 5;
    public PopupMessage ImportantPopupMessage;
    public WeaponGroups WeaponGroups;

    void Awake()
    {
        mUI = FindObjectOfType<UI>();
        mAnimatedPlayerExperience = Global.Instance.PlayerTargetExperience;
        mAnimatedPlayerMoney = Global.Instance.PlayerTargetMoney;
        mAnimatedPlayerHealth = Global.Instance.PlayerTargetHealth;
    }

    public void BeginGameplay()
    {
        mLevel = FindObjectOfType<Level>();
        mPlayer = FindObjectOfType<Player>();
        mCurrentPOI = 0;
        mState = State.EnterPOI;

        mAllCharacters = FindObjectsOfType<Character>();
        mAllEnemies = FindObjectsOfType<Enemy>();

        foreach (var enemy in mAllEnemies)
            enemy.gameObject.SetActive(false);

        mUI.SetStateAndHudMode(UI.State.Game, UI.HudMode.Hidden);
        EquipWeapon(Global.Instance.EquippedWeapon);
    }

    void RunToNextPOI()
    {
        ++mCurrentPOI;
        mPlayer.Character.SetAnimation(Character.Animation.Run);
        mState = (mLevel.POIs[mCurrentPOI].Type == POIType.EndOfLevel
            ? State.RunTowardsEndOfLevel : State.RunTowardsPOI);
    }

    public void LevelUp()
    {
        bool wasFullHealth = (mAnimatedPlayerHealth == Global.Instance.MaxHealth);

        Global.Instance.LevelBaseExperience = NextExperience;
        Global.Instance.PlayerLevel = Global.Instance.PlayerLevel + 1;

        Global.Instance.MaxHealth += Global.Instance.PlayerLevel * 10;
        Global.Instance.PlayerTargetHealth = Global.Instance.MaxHealth;
        if (wasFullHealth)
            mAnimatedPlayerHealth = Global.Instance.MaxHealth;

        mAnimatedPlayerExperience = 0;
        Global.Instance.PlayerTargetExperience -= Global.Instance.LevelBaseExperience;
        if (Global.Instance.PlayerTargetExperience < 0)
            Global.Instance.PlayerTargetExperience = 0;

        ImportantPopupMessage.ShowMessage(Language.Current.LevelUp, Color.green);
    }

    public void AdjustHealth(long value)
    {
        Global.Instance.PlayerTargetHealth += value;
        if (Global.Instance.PlayerTargetHealth > Global.Instance.MaxHealth)
            Global.Instance.PlayerTargetHealth = Global.Instance.MaxHealth;
        if (Global.Instance.PlayerTargetHealth < 0)
            Global.Instance.PlayerTargetHealth = 0;

        var sign = (value >= 0 ? '+' : '-');
        var color = (value >= 0 ? new Color32(67, 183, 67, 255) : new Color32(231, 39, 45, 255));
        var message = $"{sign}{UI.FormatBigNumber(Math.Abs(value))}";
        mUI.HealthPopupMessage.ShowMessage(message, color);
        mPlayer.Character.PopupMessage.ShowMessage(message, color);
    }

    public void AdjustExperience(long value)
    {
        if (value < 0) {
            Debug.LogError("Attempted to decrease experience.");
            return;
        }

        Global.Instance.PlayerTargetExperience += value;

        var message = $"+{UI.FormatBigNumber(value)}";
        mUI.ExperiencePopupMessage.ShowMessage(message, new Color32(67, 183, 67, 255));
    }

    public void AdjustMoney(long value)
    {
        Global.Instance.PlayerTargetMoney += value;
        if (Global.Instance.PlayerTargetMoney < 0)
            Global.Instance.PlayerTargetMoney = 0;

        var sign = (value >= 0 ? '+' : '-');
        var color = (value >= 0 ? new Color32(67, 183, 67, 255) : new Color32(231, 39, 45, 255));
        var message = $"{sign}{UI.FormatBigNumber(Math.Abs(value))}";
        mUI.MoneyPopupMessage.ShowMessage(message, color);
    }

    public void EquipWeapon(InventoryItem weapon)
    {
        Global.Instance.EquippedWeapon = weapon;

        InventoryItemWeaponSpec spec = null;
        if (weapon != null)
            spec = weapon.Spec as InventoryItemWeaponSpec;

        mPlayer.SetWeapon(spec);
    }

    void UpdateSelectedEnemyVisual()
    {
        int i = 0;
        foreach (var enemy in mEnemies) {
            enemy.SetArrowVisible(mState == State.PlayerInput && i == mSelectedEnemy);
            ++i;
        }
    }

    int GetEnemyAttackCoefficient()
    {
        if (Global.Instance.MaxHealth <= 120)
            return 1;
        else if (Global.Instance.MaxHealth <= 150)
            return 2;
        else if (Global.Instance.MaxHealth <= 200)
            return 3;
        else if (Global.Instance.MaxHealth <= 250)
            return 5;
        else
            return 10;
    }

    bool IsPlayerCharacter(Character c)
    {
        // FIXME: hack hack hack
        return (c.gameObject == mPlayer.gameObject);
    }

    void ApplyWeaponToCharacter(Character character, InventoryItemWeaponSpec weapon, long damage)
    {
        // FIXME: miss

        if (!IsPlayerCharacter(character))
            character.GetComponent<Enemy>().ApplyDamage(damage);
        else {
            AdjustHealth(-damage);
            if (Health <= 0)
                Global.Instance.LoadScene("Lose");
        }
    }

    bool MoveCharacter(Character character, Vector3 targetPosition, float stopDistance = 0.0f)
    {
        var t = character.transform;
        var currentPosition = t.position;

        Vector3 direction = targetPosition - currentPosition;
        float magnitude = direction.magnitude;
        direction /= magnitude;

        magnitude -= stopDistance;
        if (magnitude > character.RunSpeed) {
            t.position = currentPosition + direction * character.RunSpeed;
            return false;
        } else {
            t.position = targetPosition - direction * stopDistance;
            return true;
        }
    }

    void AnimateMeleeAttack(Character targetCharacter)
    {
        var character = mCharacters[mCurrentCharacter];
        mTargetCharacter = targetCharacter;

        if (Weapon.IsRanged(character.EquippedWeapon)) {
            character.SetAnimation(Character.Animation.RangedIdle);
            mState = State.RangedAttackCharge;
            mRangedAttackChargeTime = 0.2f;
        } else {
            character.SetAnimation(Character.Animation.Run);
            mState = State.MeleeRunToEnemy;
        }
    }

    void PerformAttackDamage()
    {
        var character = mCharacters[mCurrentCharacter];

        if (IsPlayerCharacter(character)) {
            var weapon = EquippedWeapon?.Spec as InventoryItemWeaponSpec;
            long damage = weapon?.BaseDamage ?? 1;
            ApplyWeaponToCharacter(mTargetCharacter, weapon, damage);
        } else {
            var weapon = WeaponGroups.WeaponFromEnum(character.WeaponVisual)?.WithLevel(0);
            if (weapon != null) {
                // FIXME
                long damage = weapon.BaseDamage * 4;//(GetEnemyAttackCoefficient() + Global.Instance.MaxHealth * 5 / 100);
                ApplyWeaponToCharacter(mTargetCharacter, weapon, damage);
            }
        }
    }

    void PerformThrowDamage()
    {
        var character = mCharacters[mCurrentCharacter];

        if (IsPlayerCharacter(character)) {
            // FIXME
            long damage = 5;//weapon?.BaseDamage ?? 1;
            //ApplyWeaponToCharacter(mTargetCharacter, weapon, damage);
            mTargetCharacter.GetComponent<Enemy>().ApplyDamage(damage);
        } else {
            var weapon = WeaponGroups.WeaponFromEnum(character.WeaponVisual)?.WithLevel(0);
            if (weapon != null) {
                // FIXME
                long damage = 5;//(GetEnemyAttackCoefficient() + Global.Instance.MaxHealth * 5 / 100);
                ApplyWeaponToCharacter(mTargetCharacter, weapon, damage);
            }
        }
    }

    void NextCharacter()
    {
        do {
            mCurrentCharacter = (mCurrentCharacter + 1) % mCharacters.Count;
        } while (mCharacters[mCurrentCharacter] == null);
    }

    void Update()
    {
        switch (mState) {
            case State.Loading: StateLoading(); break;
            case State.EnterPOI: StateEnterPOI(); break;
            case State.RunTowardsPOI: StateRunTowardsPOI(); break;
            case State.RunTowardsEndOfLevel: StateRunTowardsEndOfLevel(); break;
            case State.WaitShopClosed: StateWaitShopClosed(); break;
            case State.BeginFight: StateBeginFight(); break;
            case State.WaitAppear: StateWaitAppear(); break;
            case State.PlayerInput: StatePlayerInput(); break;
            case State.EnemyMove: StateEnemyMove(); break;
            case State.MeleeRunToEnemy: StateMeleeRunToEnemy(); break;
            case State.MeleeAttack: StateMeleeAttack(); break;
            case State.MeleeRunBack: StateMeleeRunBack(); break;
            case State.RangedAttackCharge: StateRangedAttackCharge(); break;
            case State.RangedAttack: StateRangedAttack(); break;
            case State.ThrowAttack: StateThrowAttack(); break;
            case State.AttackEnded: StateAttackEnded(); break;
            case State.CheckWinCondition: StateCheckWinCondition(); break;
            case State.Ended: break;
        }

        Util.Animate(ref mAnimatedPlayerHealth, Global.Instance.PlayerTargetHealth, HealthAnimateSpeed);
        Util.Animate(ref mAnimatedPlayerExperience, Global.Instance.PlayerTargetExperience, ExperienceAnimateSpeed);
        Util.Animate(ref mAnimatedPlayerMoney, Global.Instance.PlayerTargetMoney, MoneyAnimateSpeed);

        if (mAnimatedPlayerExperience >= NextExperience)
            LevelUp();
    }

    void StateLoading()
    {
    }

    void StateEnterPOI()
    {
        if (mLevel == null || mLevel.POIs == null)
            return;

        var poi = mLevel.POIs[mCurrentPOI];
        mLevel.SetToPOI(poi);
        switch (poi.Type) {
            case POIType.Start:
                RunToNextPOI();
                break;

            case POIType.Fight:
                mState = State.BeginFight;
                break;

            case POIType.Shop:
                mUI.OpenShop();
                mState = State.WaitShopClosed;
                break;

            case POIType.Prize:
                // FIXME
                RunToNextPOI();
                break;

            case POIType.EndOfLevel:
                Global.Instance.LoadScene(poi.NextLevel);
                mState = State.Ended;
                break;
        }
    }

    void StateRunTowardsPOI()
    {
        if (!mLevel.MoveTowardsPOI(mLevel.POIs[mCurrentPOI], 4.0f * Time.deltaTime)) // FIXME: magic constant
            return;

        mPlayer.Character.SetAnimation(Character.Animation.Idle, 0.1f);
        mState = State.EnterPOI;
    }

    void StateRunTowardsEndOfLevel()
    {
        var t = mPlayer.transform;
        var position = t.position;

        Vector3 target = mLevel.POIs[mCurrentPOI].Position;
        Vector3 delta = target - position;

        float speed = 8.0f * Time.deltaTime;
        if (delta.magnitude > speed)
            position += delta.normalized * speed;
        else {
            position = target;
            mPlayer.Character.SetAnimation(Character.Animation.Idle, 0.1f);
            mState = State.EnterPOI;
        }

        t.position = position;
    }

    void StateWaitShopClosed()
    {
        if (mUI.CurrentState == UI.State.Shop)
            return;

        RunToNextPOI();
    }

    void StateBeginFight()
    {
        mUI.SetHudMode(UI.HudMode.Passive);

        mCharacters = new List<Character>(mAllCharacters);
        mEnemies = new List<Enemy>(mAllEnemies);

        foreach (var enemy in mEnemies) {
            enemy.gameObject.SetActive(true);
            enemy.Randomize();
        }

        mSelectedEnemy = 0;
        UpdateSelectedEnemyVisual();

        mCurrentCharacter = 0;
        foreach (var c in mCharacters) {
            if (IsPlayerCharacter(c))
                break;
            ++mCurrentCharacter;
        }

        mState = State.WaitAppear;
    }

    void StateWaitAppear()
    {
        foreach (var enemy in mEnemies) {
            if (enemy.IsAppearing)
                return;
        }

        mState = State.CheckWinCondition;
    }

    void StatePlayerInput()
    {
    }

    void StateEnemyMove()
    {
        // FIXME

        AnimateMeleeAttack(mPlayer.Character);
    }

    void StateMeleeRunToEnemy()
    {
        var character = mCharacters[mCurrentCharacter];
        if (MoveCharacter(character, mTargetCharacter.transform.position, character.AttackDistance)) {
            character.SetAnimation(Character.Animation.MeleeAttack);
            mMeleeAttackTime = 0.4f;
            mMeleeDidDamage = false;
            mState = State.MeleeAttack;
        }
    }

    void StateMeleeAttack()
    {
        if (!mMeleeDidDamage) {
            mMeleeAttackTime -= Time.deltaTime;
            if (mMeleeAttackTime <= 0.0f) {
                mMeleeDidDamage = true;
                PerformAttackDamage();
            }
        }

        var character = mCharacters[mCurrentCharacter];
        if (!character.IsPlayingAnimation()) {
            mState = State.MeleeRunBack;
            character.SetAnimation(Character.Animation.Run, 0.2f);
            character.SetFlipHorizontally(true);
            if (!mMeleeDidDamage)
                PerformAttackDamage();
        }
    }

    void StateMeleeRunBack()
    {
        var character = mCharacters[mCurrentCharacter];
        if (MoveCharacter(character, character.OriginalPosition)) {
            character.SetAnimation(Character.Animation.Idle, 0.2f);
            character.SetFlipHorizontally(false);
            mState = State.AttackEnded;
        }
    }

    void StateRangedAttackCharge()
    {
        mRangedAttackChargeTime -= Time.deltaTime;
        if (mRangedAttackChargeTime > 0.0f)
            return;

        mRangedAttackTime = 0.5f;
        mRangedDidDamage = false;
        mState = State.RangedAttack;

        var character = mCharacters[mCurrentCharacter];
        character.ShootProjectile(mTargetCharacter.transform.position, 0.5f);
        character.SetAnimation(Character.Animation.RangedAttack);
    }

    void StateRangedAttack()
    {
        if (!mRangedDidDamage) {
            mRangedAttackTime -= Time.deltaTime;
            if (mRangedAttackTime <= 0.0f) {
                mRangedDidDamage = true;
                PerformAttackDamage();
            }
        }

        var character = mCharacters[mCurrentCharacter];
        if (!character.IsPlayingAnimation()) {
            mState = State.AttackEnded;
            character.SetAnimation(Character.Animation.Idle, 0.2f);
            if (!mRangedDidDamage)
                PerformAttackDamage();
        }
    }

    void StateThrowAttack()
    {
        mThrowAttackTime -= Time.deltaTime;
        if (mThrowAttackTime <= 0.0f) {
            PerformThrowDamage();
            mState = State.AttackEnded;
        }
    }

    void StateAttackEnded()
    {
        NextCharacter();
        mState = State.CheckWinCondition;
    }

    void StateCheckWinCondition()
    {
        int n = mEnemies.Count;
        while (n-- > 0) {
            var enemy = mEnemies[n];
            if (enemy.Health <= 0) {
                mEnemies.RemoveAt(n);
                Util.SetArrayItemToNull(mCharacters, enemy.Character);
            }
        }

        if (mEnemies.Count == 0) {
            foreach (var enemy in mAllEnemies) {
                if (enemy.IsPlayingDeathAnimation() && enemy.gameObject.activeSelf)
                    return;
            }
            RunToNextPOI();
        } else {
            if (mCharacters[mCurrentCharacter] == null)
                NextCharacter();

            mSelectedEnemy = mSelectedEnemy % mEnemies.Count;

            if (!IsPlayerCharacter(mCharacters[mCurrentCharacter]))
                mState = State.EnemyMove;
            else {
                mState = State.PlayerInput;
                mUI.SetHudMode(UI.HudMode.Active);
            }

            UpdateSelectedEnemyVisual();
        }
    }

    public void SwitchToNextEnemy()
    {
        if (mState != State.PlayerInput)
            return;

        mSelectedEnemy = (mSelectedEnemy + 1) % mEnemies.Count;
        UpdateSelectedEnemyVisual();
    }

    public void PerformMeleeAttack()
    {
        if (mState != State.PlayerInput)
            return;

        AnimateMeleeAttack(mEnemies[mSelectedEnemy].Character);
        mUI.SetHudMode(UI.HudMode.Passive);
        UpdateSelectedEnemyVisual();
    }

    public void PerformThrowAttack(Throwable.Visual visual)
    {
        if (mState != State.PlayerInput)
            return;

        var character = mCharacters[mCurrentCharacter];
        mTargetCharacter = mEnemies[mSelectedEnemy].Character;

        Throwable throwable = Instantiate(ThrowablePrefab);
        throwable.SetVisual(visual);

        var projectile = throwable.gameObject.AddComponent<Projectile>();
        projectile.DeleteWhenDone = true;
        projectile.transform.SetParent(character.ThrowableShooter.transform, false);
        projectile.BeginAnimating(mTargetCharacter.transform.position, 0.5f);

        mThrownItem = visual;

        mThrowAttackTime = 0.5f;
        mState = State.ThrowAttack;

        mUI.SetHudMode(UI.HudMode.Passive);
        UpdateSelectedEnemyVisual();
    }
}
