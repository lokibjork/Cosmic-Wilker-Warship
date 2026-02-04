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
        if (playerTarget == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer <= attackRange)
        {
            // --- CORREÇÃO AQUI ---
            if (lookAtPlayer)
            {
                AimAtPlayer();
            }
            else
            {
                // Se não está a mirar no player, força a mira para a ESQUERDA (180 graus)
                // Podes mudar para 0f se quiseres que atire para a direita
                firePoint.rotation = Quaternion.Euler(0, 0, 180f);
            }
            // ---------------------

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

        // Se for 1 bala, o ângulo é 0. Se forem mais, calculamos o passo.
        float startAngle = -weaponData.spreadAngle / 2f;
        float angleStep = weaponData.projectilesPerShot > 1
            ? weaponData.spreadAngle / (weaponData.projectilesPerShot - 1)
            : 0;

        // Loop para criar cada bala do leque
        for (int i = 0; i < weaponData.projectilesPerShot; i++)
        {
            // Calcula a rotação desta bala específica
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, currentAngle);

            Instantiate(weaponData.projectilePrefab, firePoint.position, rotation);
        }
    }

    // Desenha o alcance no editor para facilitar
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}