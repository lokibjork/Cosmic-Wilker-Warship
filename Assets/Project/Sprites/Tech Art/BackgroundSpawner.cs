using UnityEngine;
using System.Collections;

public class BackgroundSpawner : MonoBehaviour
{
    [Header("O que Spawnar")]
    [Tooltip("Arraste aqui seus prefabs de estrelas e asteroides.")]
    public GameObject[] spawnPrefabs;

    [Header("Tempo")]
    [Tooltip("Intervalo mínimo e máximo entre cada spawn.")]
    public Vector2 spawnIntervalRange = new Vector2(0.5f, 2f);

    [Header("Área de Spawn (Firepoints)")]
    [Tooltip("Tamanho da caixa onde os objetos podem nascer.")]
    public Vector2 spawnAreaSize = new Vector2(1f, 10f);
    [Tooltip("Deslocamento do centro da caixa em relação a este objeto.")]
    public Vector2 spawnAreaOffset = new Vector2(10f, 0f);

    private void Start()
    {
        if (spawnPrefabs.Length == 0)
        {
            Debug.LogError("BackgroundSpawner: Nenhum prefab atribuído!");
            return;
        }

        // Inicia a rotina de spawn infinito
        StartCoroutine(SpawnRoutine());
    }

    // Corrotina para gerenciar o tempo de spawn
    IEnumerator SpawnRoutine()
    {
        while (true) // Loop infinito
        {
            // 1. Espera um tempo aleatório
            float waitTime = Random.Range(spawnIntervalRange.x, spawnIntervalRange.y);
            yield return new WaitForSeconds(waitTime);

            SpawnObject();
        }
    }

    void SpawnObject()
    {
        // 2. Escolhe um prefab aleatório da lista
        int randomIndex = Random.Range(0, spawnPrefabs.Length);
        GameObject prefabToSpawn = spawnPrefabs[randomIndex];

        // 3. Define o "Firepoint" aleatório
        // Calcula uma posição aleatória dentro da caixa definida pelas variáveis de Área
        Vector3 center = transform.position + (Vector3)spawnAreaOffset;
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        Vector3 spawnPos = center + new Vector3(randomX, randomY, 0);

        // 4. Cria o objeto na posição calculada
        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
    }

    // --- Ferramenta Visual para o Editor ---
    // Isso desenha uma caixa na Scene view para você ver onde a área de spawn está
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + (Vector3)spawnAreaOffset;
        Gizmos.DrawWireCube(center, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 1));
    }
}