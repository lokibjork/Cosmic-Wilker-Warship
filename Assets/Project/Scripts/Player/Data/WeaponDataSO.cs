using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "CosmicWilker/Weapons/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Configuração Visual")]
    public GameObject projectilePrefab; // O prefab da bala que criamos acima

    [Header("Configuração de Combate")]
    public float fireRate = 0.2f; // Tempo entre tiros (menor = mais rápido)
    public int damage = 1;

    [Header("Padrão de Tiro")]
    public int projectilesPerShot = 1; // Quantas balas saem de uma vez
    public float spreadAngle = 0f;     // Ângulo de abertura (ex: 45 graus)
}