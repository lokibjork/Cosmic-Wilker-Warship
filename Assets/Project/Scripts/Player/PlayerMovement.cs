using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Dependências")]
    [SerializeField] private PlayerStatsSO stats;

    // 1. Adicionamos a referência ao Dash (opcional, pois podemos usar GetComponent)
    private PlayerDash playerDash;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // 2. Tentamos encontrar o componente Dash no mesmo objeto automaticamente
        playerDash = GetComponent<PlayerDash>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        // 3. A GUARDA DE SEGURANÇA:
        // Se o componente de Dash existir E estivermos no meio de um dash...
        // ...NÃO fazemos nada e deixamos o script de Dash controlar a física.
        if (playerDash != null && playerDash.IsDashing())
        {
            return;
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 targetVelocity = moveInput * stats.moveSpeed;
        rb.linearVelocity = targetVelocity;
    }
}