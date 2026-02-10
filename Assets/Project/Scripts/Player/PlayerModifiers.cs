using UnityEngine;

public class PlayerModifiers : MonoBehaviour
{
    [Header("Ataque - Projéteis")]
    public int bonusProjectiles = 0; // Multishot (GDD: Até +2 para totalizar 3x)
    public bool hasFireAmmo = false; // GDD: Elemental Fogo
    public bool hasLightningAmmo = false; // GDD: Elemental Raio

    [Header("Sobrevivência")]
    public float vampirismChance = 0f; // GDD: 0.05, 0.10, 0.20
    public bool hasSuperNova = false;  // GDD: Revive 1 vez

    // Função auxiliar para resetar (caso queiras reiniciar a run sem recarregar a cena)
    public void ResetModifiers()
    {
        bonusProjectiles = 0;
        hasFireAmmo = false;
        hasLightningAmmo = false;
        vampirismChance = 0f;
        hasSuperNova = false;
    }
}