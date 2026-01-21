using UnityEngine;

public class BlackHoleProjectile : ProjectileBase
{
    [Header("Propriedades do Vórtice")]
    [SerializeField] private float pullRadius = 5f;
    [SerializeField] private float pullForce = 10f;

    // Sobrescrevemos o Start para ele ser mais lento (buracos negros viajam devagar)
    protected override void Start()
    {
        base.Start(); // Mantém o código de destruir por tempo
        rb.linearVelocity = transform.right * (speed * 0.2f); // 20% da velocidade normal
    }

    protected override void OnHit(Collider2D target)
    {
        // Buracos negros geralmente ignoram colisão direta simples ou explodem
        // Neste exemplo, vamos deixar vazio pois o efeito está no Update abaixo
    }

    // Podemos adicionar lógica extra que a base não tem
    private void Update()
    {
        // Encontra tudo num raio de acção
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, pullRadius);

        foreach (var obj in objectsInRange)
        {
            // Se for inimigo
            if (obj.CompareTag("Enemy") && obj.TryGetComponent(out Rigidbody2D enemyRb))
            {
                // Calcula direção para o centro do tiro
                Vector2 direction = (transform.position - obj.transform.position).normalized;
                // Puxa o inimigo
                enemyRb.AddForce(direction * pullForce * Time.deltaTime);
            }
        }
    }

    // Para desenhar o raio na tela da Unity e facilitar o ajuste
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}