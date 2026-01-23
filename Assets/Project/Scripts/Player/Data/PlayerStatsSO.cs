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
    public float dashSpeed = 25f;    // Velocidade durante o dash
    public float dashDuration = 0.2f; // Quanto tempo dura o empurrão
    public float dashCooldown = 1f;   // Tempo de espera até poder usar de novo

    [Header("Parry (Level 2)")]
    public float parryDuration = 0.2f;  // Janela de tempo para acertar o parry (muito curta!)
    public float parryCooldown = 1.5f;  // Tempo punitivo se errar
    public float parryRadius = 2f;      // Tamanho da área de proteção
}