using UnityEngine;
using UnityEngine.Events;
using MoreMountains.Feedbacks; // Se estiver usando o Feel

public class HealthSystem : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool isInvulnerable = false;

    [Header("Feedback")]
    [SerializeField] private MMF_Player hitFeedBack;

    [Header("Loot")]
    [SerializeField] private GameObject dropItemPrefab;
    [Range(0f, 1f)][SerializeField] private float dropChance = 0.5f;

    private int currentHealth;

    [Header("Eventos")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDeath;
    // Evento novo que envia o valor da vida para a UI atualizar os corações
    public UnityEvent<int> OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth); // Atualiza UI ao iniciar
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= damageAmount;

        if (hitFeedBack != null) hitFeedBack.PlayFeedbacks();



        OnTakeDamage?.Invoke();
        OnHealthChanged?.Invoke(currentHealth); // Avisa a UI

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        OnHealthChanged?.Invoke(currentHealth); // Avisa a UI
    }

    private void Die()
    {
        HandleDrop();
        OnDeath?.Invoke();
        // Destroy(gameObject); // Ou desativar, conforme tua lógica
    }

    private void HandleDrop()
    {
        if (dropItemPrefab != null && Random.value <= dropChance)
        {
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        }
    }

    public void SetInvulnerability(bool status)
    {
        isInvulnerable = status;
    }

    public int GetCurrentHealth() => currentHealth;

    // --- MÉTODO NOVO PARA UPGRADES ---
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        Heal(amount); // Cura o valor adicionado para não ficar com slot vazio
        Debug.Log($"[Upgrade] Vida Máxima Aumentada para: {maxHealth}");
    }
}