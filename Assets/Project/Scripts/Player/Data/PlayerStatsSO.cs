using UnityEngine;

// Isto cria um menu no botão direito da Unity para criar este arquivo
[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "CosmicWilker/Player/Player Stats")]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Configurações de Vida")]
    public int maxHealth = 3;

    [Header("Movimentação")]
    public float moveSpeed = 10f;

    [Header("Dash (Level 1)")]
    public float dashDistance = 3f;
    public float dashCooldown = 1f;
    public float dashDuration = 0.2f;

    // Aqui podemos adicionar mais configurações no futuro sem quebrar o código
}