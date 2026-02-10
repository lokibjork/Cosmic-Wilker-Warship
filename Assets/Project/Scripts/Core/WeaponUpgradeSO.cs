using UnityEngine;

[CreateAssetMenu(menuName = "CosmicWilker/Upgrades/Weapon Upgrade")]
public class WeaponUpgradeSO : UpgradeBaseSO
{
    [Header("Nova Arma")]
    public WeaponDataSO newWeaponData;

    public override void Apply(GameObject player)
    {
        if (player.TryGetComponent(out PlayerWeapon weaponController))
        {
            weaponController.EquipWeapon(newWeaponData);
        }

        Debug.Log($"Arma Equipada: {newWeaponData.name}");
    }
}