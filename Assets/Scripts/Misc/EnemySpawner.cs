using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public List<GameObject> enemyPrefabs;
    public int numberOfEnemiesToSpawn = 5;

    private List<Transform> spawnPoints = new List<Transform>();

    void Start()
    {
        FindSpawnPoints();
        SpawnEnemies();
    }

    void FindSpawnPoints()
    {
        spawnPoints.Clear();
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");

        foreach (GameObject obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("Nenhum ponto de spawn encontrado com a tag 'SpawnPoint'!");
        }
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Não há prefabs de inimigos ou pontos de spawn definidos!");
            return;
        }

        HashSet<int> usedSpawnPoints = new HashSet<int>();

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            int randomSpawnIndex;

            do
            {
                randomSpawnIndex = Random.Range(0, spawnPoints.Count);
            } while (usedSpawnPoints.Contains(randomSpawnIndex) && usedSpawnPoints.Count < spawnPoints.Count);

            usedSpawnPoints.Add(randomSpawnIndex);
            Transform spawnPoint = spawnPoints[randomSpawnIndex];

            Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity);
        }
    }
}
