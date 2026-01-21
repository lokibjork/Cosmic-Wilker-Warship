using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Dependências")]
    [SerializeField] private WeaponDataSO currentWeapon; // Arrasta o BasicWeapon aqui
    [SerializeField] private Transform firePoint; // Local de onde sai o tiro (boca da arma)

    private bool isFiring;
    private float nextFireTime;

    // Input System: Chamado quando apertas ou soltas o botão de ataque
    public void OnFire(InputValue value)
    {
        isFiring = value.isPressed;
    }

    private void Update()
    {
        // Se o botão está apertado E o tempo atual é maior que o tempo do próximo tiro
        if (isFiring && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (currentWeapon == null || currentWeapon.projectilePrefab == null)
        {
            Debug.LogWarning("Arma ou Prefab do projétil não configurados!");
            return;
        }

        // Atualiza o tempo para o próximo tiro
        nextFireTime = Time.time + currentWeapon.fireRate;

        // Cria a bala na posição do firePoint e com a rotação do firePoint
        Instantiate(currentWeapon.projectilePrefab, firePoint.position, firePoint.rotation);

        // Dica Pro: Futuramente, para performance comercial, usaremos "Object Pooling" aqui
        // em vez de Instantiate/Destroy repetidamente.
    }
}