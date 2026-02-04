using UnityEngine;
using UnityEngine.Events;

public class LevelSystem : MonoBehaviour
{
    [Header("Configurações de Nível")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int baseXP = 100;
    [SerializeField] private float growthFactor = 1.2f; // Cada nível precisa de 20% mais XP que o anterior

    [Header("Estado Atual")]
    [SerializeField] private int currentXP = 0;
    [SerializeField] private int xpToNextLevel;
    [SerializeField] private int pendingLevelUps = 0; // "Fichas" acumuladas para o cassino

    [Header("Eventos para UI")]
    // Envia (XP Atual, XP Total Necessário) para atualizar a barra
    public UnityEvent<int, int> OnExperienceChanged;
    // Envia (Nível Atual)
    public UnityEvent<int> OnLevelChanged;
    // Avisa que há upgrades disponíveis (para piscar o botão do menu)
    public UnityEvent<bool> OnPendingUpgradesChanged;

    private void Start()
    {
        CalculateNextLevelXP();
        UpdateUI();
    }

    private void CalculateNextLevelXP()
    {
        // Fórmula simples de RPG: Base * (Fator ^ Nível)
        xpToNextLevel = Mathf.RoundToInt(baseXP * Mathf.Pow(growthFactor, currentLevel - 1));
    }

    public void AddExperience(int amount)
    {
        currentXP += amount;

        // Checa se subiu de nível (pode subir vários de uma vez se pegar muito XP)
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        pendingLevelUps++; // Ganha uma ficha de cassino

        CalculateNextLevelXP();

        // Avisa o sistema (som de level up, brilho)
        OnLevelChanged?.Invoke(currentLevel);

        // Avisa que temos pendências (para acender o ícone do cassino)
        OnPendingUpgradesChanged?.Invoke(true);

        Debug.Log($"LEVEL UP! Nível: {currentLevel}. Fichas de Cassino: {pendingLevelUps}");
    }

    // Função que será chamada quando o jogador gastar a ficha no Cassino
    public void ConsumeUpgradeToken()
    {
        if (pendingLevelUps > 0)
        {
            pendingLevelUps--;
            if (pendingLevelUps <= 0)
            {
                OnPendingUpgradesChanged?.Invoke(false);
            }
        }
    }

    public bool HasPendingUpgrades() => pendingLevelUps > 0;

    private void UpdateUI()
    {
        OnExperienceChanged?.Invoke(currentXP, xpToNextLevel);
    }
}