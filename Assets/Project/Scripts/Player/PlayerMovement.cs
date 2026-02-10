using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Dependências")]
    [SerializeField] private PlayerStatsSO stats;

    private PlayerDash playerDash;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Variável interna para podermos modificar a velocidade sem estragar o arquivo original
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerDash = GetComponent<PlayerDash>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        // Inicializa a velocidade atual com o valor base do arquivo
        if (stats != null)
        {
            currentSpeed = stats.moveSpeed;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        // Se estiver dando Dash, o script de Dash controla a física
        if (playerDash != null && playerDash.IsDashing())
        {
            return;
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        // Usa a currentSpeed (que pode ter sido alterada por upgrades)
        Vector2 targetVelocity = moveInput * currentSpeed;
        rb.linearVelocity = targetVelocity;
    }

    // --- MÉTODO NOVO PARA UPGRADES ---
    public void ModifySpeed(float amountToAdd)
    {
        currentSpeed += amountToAdd;
        Debug.Log($"[Upgrade] Nova Velocidade: {currentSpeed}");
    }
}