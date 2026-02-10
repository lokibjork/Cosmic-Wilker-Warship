using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CosmicWilker/Systems/Upgrade Pool")]
public class UpgradePoolSO : ScriptableObject
{
    [Header("Todas as Cartas do Jogo")]
    public List<UpgradeBaseSO> allUpgrades;

    public List<UpgradeBaseSO> GetUpgradesByRarity(Rarity rarity)
    {
        List<UpgradeBaseSO> filtered = new List<UpgradeBaseSO>();
        foreach (var upgrade in allUpgrades)
        {
            if (upgrade.rarity == rarity) filtered.Add(upgrade);
        }
        return filtered;
    }
}