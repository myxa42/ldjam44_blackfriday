
using UnityEngine;

[CreateAssetMenu(menuName = "Specs/Inventory/Weapon")]
public sealed class InventoryItemWeaponSpec : InventoryItemSpec
{
    public const int NumLevels = 3;

    public Weapon.Visual Weapon;
    public Weapon.Special Special;
    public int SpecialProbability = 1;
    public int BaseDamage = 1;
    public int[] Damage = new int[NumLevels];
    public int BaseCritProbability = 1;
    public int[] CritProbability = new int[NumLevels];
    public int[] UpgradeCost = new int[NumLevels];
    public int[] SpecialUpgradeCost = new int[NumLevels];

    public int Level { get; private set; }
    public InventoryItemWeaponSpec BaseSpec { get; private set; }
    public InventoryItemWeaponSpec[] LevelSpecs { get; private set; }

    void OnDestroy()
    {
        if (LevelSpecs != null) {
            foreach (var level in LevelSpecs) {
                if (level != null)
                    Destroy(level);
            }
        }
    }

    public bool CanUpgrade()
    {
        return Level < NumLevels;
    }

    public int GetUpgradeCost()
    {
        if (Special == global::Weapon.Special.None)
            return UpgradeCost[Level];
        else
            return SpecialUpgradeCost[Level];
    }

    public InventoryItemWeaponSpec WithLevel(int level)
    {
        if (BaseSpec != null)
            return BaseSpec.WithLevel(level);

        if (level == 0)
            return this;

        if (LevelSpecs == null || LevelSpecs.Length == 0)
            LevelSpecs = new InventoryItemWeaponSpec[NumLevels];

        var spec = LevelSpecs[level - 1];
        if (spec == null) {
            spec = Instantiate(this);
            spec.Level = level;
            spec.BaseSpec = this;
            spec.BaseDamage = spec.Damage[level - 1];
            spec.BaseCritProbability = spec.CritProbability[level - 1];
            LevelSpecs[level - 1] = spec;
        }

        return spec;
    }
}
