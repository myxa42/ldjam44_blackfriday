
using System;
using UnityEngine;

public sealed class Character : MonoBehaviour
{
    public enum Visual
    {
        Female,
        Male,
        Zombie,
    };

    public enum Animation
    {
        Idle,
        MeleeIdle,
        RangedIdle,
        Walk,
        Run,
        MeleeRun,
        RangedRun,
        MeleeAttack,
        RangedAttack,
        Death,
    }

    public GameObject Arrow;
    public GameObject LifeBarContainer;
    public GameObject ThrowableShooter;
    public float ArrowSpeed = 1.0f;
    public float ArrowRange = 0.1f;
    public PopupMessage PopupMessage;
    public float RunSpeed = 1.0f;
    public float AttackDistance = 1.0f;
    public Vector3 OriginalPosition { get; private set; }
    public Weapon.Visual EquippedWeapon => mWeapon;

    [Header("Visuals")]
    public GameObject RootVisual;
    public GameObject VisualFemale;
    public GameObject VisualMaleA;
    public GameObject VisualMaleB;
    public GameObject VisualZombie;

    [Header("Weapons")]
    public Weapon WeaponFemale;
    public Weapon WeaponMaleA;
    public Weapon WeaponMaleB;
    public Weapon WeaponZombie;

    public Weapon.Visual WeaponVisual => mWeapon;

    private Weapon mCurrentWeapon;
    private GameObject mCurrentVisual;
    private Animator mAnimator;
    private Animation mCurrentAnimation = Animation.Idle;
    private Weapon.Visual mWeapon;
    private Vector3 mBaseArrowPosition;
    private float mBaseAngle;
    private float mTime;

    void Awake()
    {
        OriginalPosition = transform.position;
        mBaseAngle = RootVisual.transform.eulerAngles.y;
        mBaseArrowPosition = Arrow.transform.position;
        mCurrentVisual = VisualMaleB;
        mCurrentWeapon = WeaponMaleB;
        WeaponFemale.SetVisual(mWeapon);
        WeaponMaleA.SetVisual(mWeapon);
        WeaponMaleB.SetVisual(mWeapon);
        WeaponZombie.SetVisual(mWeapon);
        UpdateVisual();
    }

    void Start()
    {
        Arrow.SetActive(false);
    }

    public void SetArrowVisible(bool flag)
    {
        Arrow.SetActive(flag);
    }

    public void SetAnimation(Animation animation, float transitionDuration = 0.01f)
    {
        if (mCurrentAnimation != animation) {
            mCurrentAnimation = animation;
            mAnimator.CrossFade(GetCurrentAnimationName(), transitionDuration);
        }
    }

    public void ShootProjectile(Vector3 target, float time)
    {
        mCurrentWeapon.ShootProjectile(target, time);
    }

    public void SetFlipHorizontally(bool flag)
    {
        var t = RootVisual.transform;
        var angles = t.eulerAngles;
        angles.y = (flag ? -mBaseAngle : mBaseAngle);
        t.eulerAngles = angles;
    }

  #if UNITY_EDITOR
    string DebugGetCurrentAnimationName()
    {
        foreach (var anim in Enum.GetValues(typeof(Animation))) {
            bool hasFemale = AnimationHasFemaleVersion((Animation)anim);
            string[] prefixes = (hasFemale ? new string[]{ "Male", "Female" } : new string[]{ "Male" });
            foreach (var prefix in prefixes) {
                string animName = $"{prefix}{name.ToString()}";
                if (mAnimator.GetCurrentAnimatorStateInfo(0).IsName(animName))
                    return animName;
            }
        }

        return null;
    }
  #endif

    public bool IsPlayingAnimation()
    {
        if (mAnimator.IsInTransition(0))
            return true;

        var stateInfo = mAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime < 1.0f || stateInfo.loop;
    }

    public void SetWeapon(Weapon.Visual weapon)
    {
        if (mWeapon != weapon) {
            mWeapon = weapon;
            WeaponFemale.SetVisual(mWeapon);
            WeaponMaleA.SetVisual(mWeapon);
            WeaponMaleB.SetVisual(mWeapon);
            WeaponZombie.SetVisual(mWeapon);
        }
    }

    public void SetVisual(Visual visual)
    {
        switch (visual) {
            case Visual.Female: mCurrentVisual = VisualFemale; mCurrentWeapon = WeaponFemale; break;
            case Visual.Male: mCurrentVisual = VisualMaleA; mCurrentWeapon = WeaponMaleA; break;
            case Visual.Zombie: mCurrentVisual = VisualZombie; mCurrentWeapon = WeaponZombie; break;
        }

        UpdateVisual();
    }

    public void SetRandomVisual()
    {
        var values = (Visual[])System.Enum.GetValues(typeof(Visual));
        SetVisual(values[UnityEngine.Random.Range(0, values.Length)]);
    }

    bool AnimationHasFemaleVersion(Animation anim)
    {
        return anim != Animation.RangedIdle
            && anim != Animation.RangedRun
            && anim != Animation.RangedAttack;
    }

    string GetCurrentAnimationName()
    {
        string prefix = (mCurrentVisual == VisualFemale
            && AnimationHasFemaleVersion(mCurrentAnimation) ? "Female" : "Male");
        return $"Base Layer.{prefix}{mCurrentAnimation.ToString()}";
    }

    void Update()
    {
        mTime += Time.deltaTime;
        if (mTime > 2.0f * Mathf.PI)
            mTime -= 2.0f * Mathf.PI;
        Arrow.transform.position = mBaseArrowPosition + Vector3.up * Mathf.Sin(mTime * ArrowSpeed) * ArrowRange;
    }

    void UpdateVisual()
    {
        VisualFemale.SetActive(mCurrentVisual == VisualFemale);
        VisualMaleA.SetActive(mCurrentVisual == VisualMaleA);
        VisualMaleB.SetActive(mCurrentVisual == VisualMaleB);
        VisualZombie.SetActive(mCurrentVisual == VisualZombie);

        mAnimator = mCurrentVisual.GetComponent<Animator>();
        mAnimator.Play(GetCurrentAnimationName());
    }
}
