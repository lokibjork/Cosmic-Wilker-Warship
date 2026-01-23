using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks; // O teu plugin de efeitos!

public class PlayerParry : MonoBehaviour
{
    [Header("Dependências")]
    [SerializeField] private PlayerStatsSO stats;
    [SerializeField] private HealthSystem healthSystem;

    [Header("Configuração de Alvos")]
    [Tooltip("Quais layers o Parry consegue bloquear? Selecione EnemyProjectile e Enemy.")]
    [SerializeField] private LayerMask parryTargets;

    [Header("Feedback (FEEL)")]
    [SerializeField] private MMF_Player parrySuccessFeedback; // Som/Partícula de sucesso
    [SerializeField] private MMF_Player parryMissFeedback;    // Som de "wiff" (errou)

    private bool isParrying;
    private bool canParry = true;

    private void Awake()
    {
        if (healthSystem == null) healthSystem = GetComponent<HealthSystem>();
    }

    // Input System (Botão RB)
    public void OnParry(InputValue value)
    {
        if (value.isPressed && canParry && !isParrying)
        {
            StartCoroutine(PerformParry());
        }
    }

    private IEnumerator PerformParry()
    {
        canParry = false;
        isParrying = true;

        // 1. Torna o jogador invulnerável durante o Parry?
        // Depende do design. Geralmente sim, para evitar tomar dano no frame exato.
        healthSystem.SetInvulnerability(true);

        // Toca animação ou som de tentativa (opcional)
        // animator.SetTrigger("Parry"); 

        float timer = 0f;
        bool successfulParry = false;

        // 2. Loop de verificação durante a duração do Parry (Janela Ativa)
        while (timer < stats.parryDuration)
        {
            // Cria um círculo invisível e vê o que tem dentro
            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, stats.parryRadius, parryTargets);

            foreach (var obj in hitObjects)
            {
                // Se pegamos algo perigoso
                if (obj != null)
                {
                    HandleParrySuccess(obj);
                    successfulParry = true;
                }
            }

            timer += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        // 3. Fim da janela de Parry
        isParrying = false;
        healthSystem.SetInvulnerability(false);

        // Feedback se errou (apertou à toa)
        if (!successfulParry && parryMissFeedback != null)
        {
            parryMissFeedback.PlayFeedbacks();
        }

        // 4. Cooldown
        yield return new WaitForSeconds(stats.parryCooldown);
        canParry = true;
    }

    private void HandleParrySuccess(Collider2D enemyObject)
    {
        // 1. Feedback Visual/Sonoro (FEEL)
        if (parrySuccessFeedback != null)
        {
            parrySuccessFeedback.PlayFeedbacks();
        }

        Debug.Log($"PARRY PERFECTO! Devolvendo tiro de {enemyObject.name}!");

        // Tenta pegar o corpo físico do projétil
        if (enemyObject.TryGetComponent(out Rigidbody2D projRb))
        {
            // A. INVERTER VELOCIDADE
            // Multiplicamos por -1.5f para voltar mais rápido e forte (Game Feel)
            projRb.linearVelocity = -projRb.linearVelocity * 1.5f;

            // B. MUDAR DE TIME (LAYER)
            // Muda a layer para "PlayerProjectile" para ele poder acertar inimigos
            // Certifica-te que escreveste o nome da layer exatamente igual na Unity
            enemyObject.gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");

            // C. RODAR O VISUAL
            // Gira 180 graus para a bala apontar para trás
            enemyObject.transform.Rotate(0, 0, 180f);

            // D. RESETAR TEMPO DE VIDA (Opcional)
            // Se a bala estava quase a sumir, damos mais 2 segundos de vida para ela chegar ao inimigo
            // (Requer acesso a uma função pública no ProjectileBase se quiseres fazer direito, 
            // mas por agora a velocidade extra deve resolver).
        }
        else
        {
            // Se for um objeto sem física (ex: um laser de luz), destruímos
            Destroy(enemyObject.gameObject);
        }
    }

    // Desenha o raio do Parry no editor para veres o tamanho
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stats.parryRadius);
    }
}