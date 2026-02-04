using UnityEngine;

public class ExperienceGem : MonoBehaviour
{
    [Header("Valores")]
    [SerializeField] private int xpValue = 10;

    [Header("Magnetismo")]
    [SerializeField] private float magnetRange = 5f;
    [SerializeField] private float moveSpeed = 15f;

    private Transform playerTransform;
    private bool isMagnetized = false;

    private void Update()
    {
        if (playerTransform == null)
        {
            // Tenta achar o player se ainda não achou
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
            return;
        }

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Se entrou no raio, ativa o ímã
        if (distance < magnetRange)
        {
            isMagnetized = true;
        }

        // Se foi magnetizado, voa até o player
        if (isMagnetized)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

            // Se encostou, coleta
            if (distance < 0.5f)
            {
                Collect();
            }
        }
    }

    private void Collect()
    {
        // Precisamos do sistema de Level no Player para entregar o XP
        if (playerTransform.TryGetComponent(out LevelSystem levelSystem))
        {
            levelSystem.AddExperience(xpValue);
        }

        // Efeito sonoro/partícula aqui (FEEL)
        Destroy(gameObject);
    }
}