using UnityEngine;

[CreateAssetMenu(menuName = "CosmicWilker/Upgrades/Stat Upgrade")]
public class StatUpgradeSO : UpgradeBaseSO
{
    public enum StatType
    {
        MoveSpeed,
        MaxHealth,
        FireRate,
        DashCooldown
    }

    [Header("Configuração")]
    public StatType targetStat;
    public float valueToAdd; // Pode ser negativo para reduzir cooldowns

    public override void Apply(GameObject player)
    {
        // Aqui usamos GetComponent para achar o script certo e mudar o valor
        // NOTA: Para isto funcionar bem, precisamos que os teus scripts tenham métodos públicos para alterar valores!

        switch (targetStat)
        {
            case StatType.MoveSpeed:
                if (player.TryGetComponent(out PlayerMovement movement))
                {
                    movement.ModifySpeed(valueToAdd);
                }
                break;

            case StatType.MaxHealth:
                if (player.TryGetComponent(out HealthSystem health))
                {
                    health.IncreaseMaxHealth((int)valueToAdd);
                }
                break;

            case StatType.FireRate:
                if (player.TryGetComponent(out PlayerWeapon weapon))
                {
                    weapon.ModifyFireRate(valueToAdd);
                }
                break;
        }

        Debug.Log($"Upgrade Aplicado: {upgradeName} ({targetStat} += {valueToAdd})");
    }
}