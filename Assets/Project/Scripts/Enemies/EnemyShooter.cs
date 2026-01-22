using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Configurações da Arma")]
    [SerializeField] private WeaponDataSO weaponData;
    [SerializeField] private Transform firePoint;

    [Header("Comportamento")]
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private bool lookAtPlayer = true; // Se false, atira só para a esquerda

    private Transform playerTarget;
    private float nextFireTime;

    private void Start()
    {
        // Encontra o Player automaticamente pela Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
    }

    private void Update()
    {
        // Se o player morreu ou não existe, não faz nada
        if (playerTarget == null) return;

        // Verifica a distância
        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            // 1. Mirar (Opcional)
            if (lookAtPlayer)
            {
                AimAtPlayer();
            }

            // 2. Atirar
            if (Time.time >= nextFireTime)
            {
                Shoot();
            }
        }
    }

    private void AimAtPlayer()
    {
        // Calcula a direção
        Vector2 direction = playerTarget.position - transform.position;
        // Calcula o ângulo
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Aplica a rotação no FirePoint (não no inimigo todo, para não rodar a sprite se não quiseres)
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Shoot()
    {
        if (weaponData == null || weaponData.projectilePrefab == null) return;

        nextFireTime = Time.time + weaponData.fireRate;

        // Instancia a bala na posição e ROTAÇÃO do firepoint (que já está mirando no player)
        Instantiate(weaponData.projectilePrefab, firePoint.position, firePoint.rotation);
    }

    // Desenha o alcance no editor para facilitar
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}