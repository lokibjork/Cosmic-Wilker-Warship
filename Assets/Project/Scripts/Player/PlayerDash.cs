using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dependências")]
    [SerializeField] private PlayerStatsSO stats;
    [SerializeField] private HealthSystem healthSystem;

    // --- NOVO: Campo para arrastar o Animator ---
    [Header("Animação")]
    [SerializeField] private Animator animator;
    // ------------------------------------------

    [Header("Feedback")]
    [SerializeField] private MMF_Player dashFeedback;

    private Rigidbody2D rb;
    private Vector2 rawInput;
    private bool isDashing;
    private bool canDash = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (healthSystem == null) healthSystem = GetComponent<HealthSystem>();

        // --- NOVO: Tenta achar o Animator sozinho se você esquecer de arrastar ---
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    public void OnMove(InputValue value)
    {
        rawInput = value.Get<Vector2>();
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && canDash && !isDashing)
        {
            // Se o input de movimento for Zero, cancela.
            if (rawInput == Vector2.zero)
            {
                return;
            }

            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        // 1. Setup Inicial
        canDash = false;
        isDashing = true;

        // --- NOVO: Dispara a animação ---
        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }
        // --------------------------------

        // Liga invulnerabilidade
        if (healthSystem) healthSystem.SetInvulnerability(true);

        // Toca o feedback visual/sonoro (Feel)
        if (dashFeedback) dashFeedback.PlayFeedbacks();

        // 2. Aplica a Força
        Vector2 dashDirection = rawInput == Vector2.zero ? Vector2.right : rawInput.normalized;

        rb.linearVelocity = dashDirection * stats.dashSpeed;

        // 3. Espera o tempo do Dash
        yield return new WaitForSeconds(stats.dashDuration);

        // 4. Fim do Dash
        rb.linearVelocity = Vector2.zero;
        isDashing = false;

        // Desliga invulnerabilidade
        if (healthSystem) healthSystem.SetInvulnerability(false);

        // 5. Cooldown
        yield return new WaitForSeconds(stats.dashCooldown);
        canDash = true;
    }

    public bool IsDashing() => isDashing;
}