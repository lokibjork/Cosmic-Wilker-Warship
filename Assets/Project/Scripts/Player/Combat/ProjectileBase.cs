using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Configurações Base")]
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected bool destroyOnHit = true; // Alguns projéteis, como o Buraco Negro, podem não querer morrer ao bater

    protected Rigidbody2D rb;

    // 'virtual' permite que os filhos mudem como o Start funciona se precisarem
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    protected virtual void Start()
    {
        // Comportamento padrão de voar para frente
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignora o próprio Player para não se matar
        if (other.CompareTag("Player")) return;

        // Chama a função que os filhos vão decidir o que faz
        OnHit(other);

        // Se estiver configurado para sumir ao bater, some
        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }

    // Método Abstrato: Obriga os filhos a escreverem o código de impacto
    protected abstract void OnHit(Collider2D target);
}