using UnityEngine;
using UnityEngine.InputSystem; // Necessário para o novo Input System

[RequireComponent(typeof(Rigidbody2D))] // Garante que o objeto tem física
public class PlayerMovement : MonoBehaviour
{
    [Header("Dependências")]
    [Tooltip("Arrasta aqui o arquivo criado no Passo 2")]
    [SerializeField] private PlayerStatsSO stats;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Inicialização
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Configuração de segurança para física 2D em naves
        rb.gravityScale = 0; // Nave não cai
        rb.freezeRotation = true; // Nave não roda ao bater
    }

    // Chamado quando o jogador aperta botões (Input System)
    // Precisas configurar um Input Action Asset na Unity e linkar o evento "Move" aqui
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // FixedUpdate é usado para física (movimentação suave)
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Cálculo simples: Direção * Velocidade
        // O Time.fixedDeltaTime garante que a velocidade é igual em qualquer PC
        Vector2 targetVelocity = moveInput * stats.moveSpeed;

        // Aplicamos a velocidade ao corpo físico
        rb.linearVelocity = targetVelocity;
        // OBS: Na Unity 6, 'velocity' foi renomeado para 'linearVelocity'
    }
}