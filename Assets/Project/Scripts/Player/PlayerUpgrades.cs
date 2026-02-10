using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgrades : MonoBehaviour
{
    [System.Serializable]
    public class UpgradeSlot
    {
        public string slotName;       // Nome visual (ex: "Arma Principal")
        public UpgradeType allowedType; // Tipo aceito (ex: Weapon)
        public UpgradeBaseSO currentUpgrade; // O que está equipado
        public bool isLocked;         // Se o slot está bloqueado no início
    }

    [Header("Slots de Equipamento (Arma, Dash, Skill)")]
    [SerializeField] private List<UpgradeSlot> equipmentSlots;

    [Header("Passivas (Buffs)")]
    [SerializeField] private int maxPassives = 4;
    [SerializeField] private List<UpgradeBaseSO> passiveSlots;

    // Tenta equipar. Retorna TRUE se conseguiu, FALSE se precisa substituir.
    public bool TryEquipUpgrade(UpgradeBaseSO newUpgrade)
    {
        // CASO 1: PASSIVAS
        if (newUpgrade.type == UpgradeType.Passive)
        {
            if (passiveSlots.Count < maxPassives)
            {
                passiveSlots.Add(newUpgrade);
                newUpgrade.Apply(gameObject);
                Debug.Log($"Passiva Adicionada: {newUpgrade.upgradeName}");
                return true;
            }
            else
            {
                Debug.Log("Slots de Passiva cheios! UI deve abrir menu de troca.");
                return false;
            }
        }

        // CASO 2: EQUIPAMENTOS (Arma, Dash, Skill)
        UpgradeSlot targetSlot = equipmentSlots.Find(slot => slot.allowedType == newUpgrade.type);

        if (targetSlot != null && !targetSlot.isLocked)
        {
            // Se já tem algo, removemos o antigo automaticamente (Substituição Direta)
            // Ou retornamos false se quiseres que o jogador confirme a troca.
            // Para Weapon/Dash, geralmente a troca é automática.
            if (targetSlot.currentUpgrade != null)
            {
                targetSlot.currentUpgrade.Unapply(gameObject);
                // Opcional: Avisar o CasinoManager para devolver a carta antiga ao deck?
            }

            targetSlot.currentUpgrade = newUpgrade;
            newUpgrade.Apply(gameObject);
            Debug.Log($"Equipado no slot {targetSlot.slotName}: {newUpgrade.upgradeName}");
            return true;
        }

        Debug.LogWarning($"Nenhum slot encontrado para o tipo: {newUpgrade.type}");
        return false;
    }

    // Função usada pela UI para forçar a troca de passiva
    public void ReplacePassive(UpgradeBaseSO oldPassive, UpgradeBaseSO newPassive)
    {
        if (passiveSlots.Contains(oldPassive))
        {
            oldPassive.Unapply(gameObject);
            passiveSlots.Remove(oldPassive);

            passiveSlots.Add(newPassive);
            newPassive.Apply(gameObject);
        }
    }
}