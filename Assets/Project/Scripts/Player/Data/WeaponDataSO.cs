using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "CosmicWilker/Weapons/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Configuração Visual")]
    public GameObject projectilePrefab; // O prefab da bala que criamos acima

    [Header("Configuração de Combate")]
    public float fireRate = 0.2f; // Tempo entre tiros (menor = mais rápido)
    public int damage = 1;
}