using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Dependências")]
    [SerializeField] private WeaponDataSO currentWeapon;
    [SerializeField] private Transform firePoint;

    private bool isFiring;
    private float nextFireTime;

    // Multiplicador para buffs de velocidade de ataque (começa em 100%)
    private float fireRateMultiplier = 1f;

    public void OnFire(InputValue value)
    {
        float buttonValue = value.Get<float>();
        isFiring = buttonValue > 0.5f;
    }

    private void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (currentWeapon == null || currentWeapon.projectilePrefab == null) return;

        // Calcula o tempo do próximo tiro considerando o multiplicador
        // Se o multiplicador for 2, o tempo entre tiros cai pela metade (atira 2x mais rápido)
        float adjustedFireRate = currentWeapon.fireRate / fireRateMultiplier;
        nextFireTime = Time.time + adjustedFireRate;

        // Lógica de tiro (suporta armas com Spread/Shotgun se o SO tiver suporte, senão usa padrão)
        Instantiate(currentWeapon.projectilePrefab, firePoint.position, firePoint.rotation);
    }

    // --- MÉTODOS NOVOS PARA UPGRADES ---

    public void EquipWeapon(WeaponDataSO newWeapon)
    {
        currentWeapon = newWeapon;
        Debug.Log($"[Upgrade] Nova Arma Equipada: {newWeapon.name}");
    }

    public void ModifyFireRate(float percentageToAdd)
    {
        // Ex: Se passar 0.2f, aumenta a velocidade de ataque em 20%
        fireRateMultiplier += percentageToAdd;
        Debug.Log($"[Upgrade] Novo Multiplicador de Tiro: {fireRateMultiplier}");
    }
}