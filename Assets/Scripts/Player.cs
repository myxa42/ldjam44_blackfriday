
using UnityEngine;

public sealed class Player : MonoBehaviour
{
    public Character Character => mCharacter;
    private Character mCharacter;

    void Start()
    {
        mCharacter = GetComponent<Character>();
    }

    public void SetWeapon(InventoryItemWeaponSpec weapon)
    {
        mCharacter.SetWeapon(weapon != null ? weapon.Weapon : Weapon.Visual.None);
    }
}
