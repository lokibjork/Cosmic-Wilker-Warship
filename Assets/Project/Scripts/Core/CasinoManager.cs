using System.Collections.Generic;
using UnityEngine;

public class CasinoManager : MonoBehaviour
{
    [Header("Base de Dados")]
    [SerializeField] private UpgradePoolSO masterPool;

    // Deck atual da partida (cópia do original)
    private List<UpgradeBaseSO> availableUpgrades = new List<UpgradeBaseSO>();

    private void Awake()
    {
        InitializeDeck();
    }

    public void InitializeDeck()
    {
        availableUpgrades.Clear();
        // Clona a lista para não estragar o arquivo original
        if (masterPool != null)
            availableUpgrades.AddRange(masterPool.allUpgrades);
    }

    // Sorteia 'count' cartas aleatórias
    public List<UpgradeBaseSO> RollOptions(int count)
    {
        List<UpgradeBaseSO> options = new List<UpgradeBaseSO>();
        List<UpgradeBaseSO> tempPool = new List<UpgradeBaseSO>(availableUpgrades);

        for (int i = 0; i < count; i++)
        {
            if (tempPool.Count == 0) break;

            int randomIndex = Random.Range(0, tempPool.Count);
            options.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex); // Remove da temp para não vir repetida na mesma mão
        }
        return options;
    }

    // Chamado quando o jogador escolhe uma carta
    public void ConfirmChoice(UpgradeBaseSO chosenUpgrade)
    {
        // Remove do deck oficial (Regra de Exclusividade)
        if (availableUpgrades.Contains(chosenUpgrade))
        {
            availableUpgrades.Remove(chosenUpgrade);
        }
    }

    // Chamado se o jogador descartar uma carta antiga (Regra de Substituição)
    public void ReturnToDeck(UpgradeBaseSO oldUpgrade)
    {
        if (!availableUpgrades.Contains(oldUpgrade))
        {
            availableUpgrades.Add(oldUpgrade);
        }
    }
}