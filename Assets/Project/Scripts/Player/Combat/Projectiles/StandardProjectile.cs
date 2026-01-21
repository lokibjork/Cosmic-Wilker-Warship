using UnityEngine;

public class StandardProjectile : ProjectileBase
{
    [Header("Configurações Específicas")]
    [SerializeField] private int damageAmount = 1;
    protected override void OnHit(Collider2D target)
    {
        // Tenta pegar o componente HealthSystem do objeto que batemos
        if (target.TryGetComponent(out HealthSystem targetHealth))
        {
            // Se achou, aplica o dano
            targetHealth.TakeDamage(damageAmount);
        }

        // Se não achou (ex: bateu numa parede indestrutível), só destrói a bala mesmo
    }
}