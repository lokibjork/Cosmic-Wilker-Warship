using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveAction
    {
        public string note; // Só para te organizares no editor (ex: "Onda de Sine Waves")
        public GameObject enemyPrefab;
        public int count = 1;
        public float timeBetweenSpawns = 0.5f;
        public Transform spawnPoint; // Onde eles nascem (cria objetos vazios fora da tela direita)
        public float delayBeforeNextWave = 2f;
    }

    [SerializeField] private List<WaveAction> levelWaves;

    private void Start()
    {
        StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        // Passa por cada "Ação" configurada na lista
        foreach (var wave in levelWaves)
        {
            Debug.Log($"Iniciando Onda: {wave.note}");

            for (int i = 0; i < wave.count; i++)
            {
                if (wave.enemyPrefab != null && wave.spawnPoint != null)
                {
                    Instantiate(wave.enemyPrefab, wave.spawnPoint.position, Quaternion.identity);
                }
                yield return new WaitForSeconds(wave.timeBetweenSpawns);
            }

            // Espera antes de mandar a próxima onda (Respiro para o jogador)
            yield return new WaitForSeconds(wave.delayBeforeNextWave);
        }

        Debug.Log("Level Terminado!");
    }
}