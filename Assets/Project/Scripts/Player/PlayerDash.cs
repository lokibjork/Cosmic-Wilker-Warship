using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks; // Já que estamos usando o Feel!

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dependências")]
    [SerializeField] private PlayerStatsSO stats;
    [SerializeField] private HealthSystem healthSystem;

    [Header("Feedback")]
    [SerializeField] private MMF_Player dashFeedback; // Arrasta o Feel aqui para som/partículas

    private Rigidbody2D rb;
    private Vector2 rawInput;
    private bool isDashing;
    private bool canDash = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Se esqueceu de arrastar o HealthSystem, tenta pegar automaticamente
        if (healthSystem == null) healthSystem = GetComponent<HealthSystem>();
    }

    // Lê a direção do analógico/teclado para saber para onde dar o Dash
    public void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    // Input do Botão de Dash (LB / Shift)
    public void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        // 1. Setup Inicial
        canDash = false;
        isDashing = true;

        // Liga invulnerabilidade
        if (healthSystem) healthSystem.SetInvulnerability(true);

        // Toca o feedback visual/sonoro (Feel)
        if (dashFeedback) dashFeedback.PlayFeedbacks();

        // 2. Aplica a Força
        // Se o jogador não estiver apertando nada, dá dash para frente (Direita)
        Vector2 dashDirection = rawInput == Vector2.zero ? Vector2.right : rawInput.normalized;

        // Armazena a velocidade antiga para restaurar depois (opcional, mas fica mais fluido)
        // ou força a velocidade do dash:
        rb.linearVelocity = dashDirection * stats.dashSpeed;

        // 3. Espera o tempo do Dash (Intangível e Rápido)
        yield return new WaitForSeconds(stats.dashDuration);

        // 4. Fim do Dash
        rb.linearVelocity = Vector2.zero; // Para a nave (opcional: ou volta à velocidade normal)
        isDashing = false;

        // Desliga invulnerabilidade
        if (healthSystem) healthSystem.SetInvulnerability(false);

        // 5. Cooldown
        yield return new WaitForSeconds(stats.dashCooldown);
        canDash = true;
    }

    // Pequena função para outros scripts saberem se estamos em dash
    // Útil para o futuro "Dash Cortante"
    public bool IsDashing() => isDashing;
}