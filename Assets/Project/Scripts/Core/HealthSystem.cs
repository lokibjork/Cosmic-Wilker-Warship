using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events; // Necessário para criar eventos personalizados

public class HealthSystem : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] MMF_Player HitFeedBack;

    [Header("Loot")]
    [SerializeField] private GameObject dropItemPrefab; // Arrasta o Prefab da Gema aqui
    [Range(0f, 1f)][SerializeField] private float dropChance = 1f; // 1 = 100%

    // Variável para controle interno
    private int currentHealth;

    [Header("Eventos")]
    // Evento disparado quando leva dano (útil para piscar vermelho, tocar som)
    public UnityEvent OnTakeDamage;

    // Evento disparado quando a vida chega a zero (útil para explodir, dropar item)
    public UnityEvent OnDeath;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void HandleDrop()
    {
        if (dropItemPrefab != null && Random.value <= dropChance)
        {
            Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
        }
    }

    // Função pública para receber dano de qualquer fonte (tiro, colisão, laser)
    public void TakeDamage(int damageAmount)
    {
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} tomou {damageAmount} de dano. Vida restante: {currentHealth}");

        HitFeedBack.PlayFeedbacks();

        // Avisa quem estiver ouvindo que tomou dano
        OnTakeDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Função pública para curar (caso tenhas power-ups de vida no futuro)
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void Die()
    {
        HandleDrop(); // <-- Adiciona isto
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public void SetInvulnerability(bool status)
    {
        isInvulnerable = status;
        Debug.Log($"Invulnerabilidade: {status}");
    }
}