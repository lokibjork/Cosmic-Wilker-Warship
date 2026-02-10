using UnityEngine;

public abstract class UpgradeBaseSO : ScriptableObject
{
    [Header("Dados do Cassino")]
    public string upgradeName;
    public Rarity rarity;
    public UpgradeType type; // Define em qual slot vai entrar
    public Sprite icon;
    [TextArea] public string description;

    // Aplica o efeito
    public abstract void Apply(GameObject player);

    // Remove o efeito (Essencial para quando trocas de arma ou descartas passiva)
    public virtual void Unapply(GameObject player) { }
}