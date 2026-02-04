using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum MovementType { Linear, SineWave, Chaser }

    [Header("Comportamento")]
    [SerializeField] private MovementType moveType = MovementType.Linear;
    [SerializeField] private float speed = 5f;

    [Header("Configuração Sine Wave (Cobrinha)")]
    [SerializeField] private float frequency = 2f; // Quão rápido ele sobe e desce
    [SerializeField] private float magnitude = 2f; // Quão alto/baixo ele vai

    [Header("Configuração Chaser (Kamikaze)")]
    [SerializeField] private float rotateSpeed = 200f; // Velocidade de curva

    private Vector3 startPosition;
    private Transform playerTarget;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void Start()
    {
        // Só busca o player se for do tipo Chaser, para economizar performance
        if (moveType == MovementType.Chaser)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTarget = player.transform;
        }
    }

    private void FixedUpdate()
    {
        switch (moveType)
        {
            case MovementType.Linear:
                MoveLinear();
                break;
            case MovementType.SineWave:
                MoveSineWave();
                break;
            case MovementType.Chaser:
                MoveChaser();
                break;
        }

        // Destroi se sair muito da tela (Otimização)
        if (transform.position.x < -25f || transform.position.y > 15f || transform.position.y < -15f)
        {
            Destroy(gameObject);
        }
    }

    // 1. Só anda para a esquerda
    private void MoveLinear()
    {
        rb.linearVelocity = Vector2.left * speed;
    }

    // 2. Anda em ondas (Estilo Zero Ranger / Gradius)
    private void MoveSineWave()
    {
        // Move para a esquerda
        Vector2 pos = transform.position;
        pos.x -= speed * Time.fixedDeltaTime;

        // Calcula a altura baseada no tempo (Seno)
        // Usamos Time.time para sincronizar todos, ou Time.time + ID para desincronizar
        float sinOffset = Mathf.Sin(Time.time * frequency) * magnitude;
        pos.y = startPosition.y + sinOffset;

        rb.MovePosition(pos);
    }

    // 3. Persegue o jogador (Míssil ou Kamikaze)
    private void MoveChaser()
    {
        if (playerTarget == null)
        {
            MoveLinear(); // Se player morreu, segue reto
            return;
        }

        Vector2 direction = (Vector2)playerTarget.position - rb.position;
        direction.Normalize();

        // Calcula quanto precisa rodar para olhar pro player
        float rotateAmount = Vector3.Cross(direction, transform.right).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
        rb.linearVelocity = transform.right * speed;
    }
}