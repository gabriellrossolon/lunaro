using System.Collections.Generic;
using UnityEngine;

public class DungeonSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public List<GameObject> enemyPrefabs;
    public int numberOfEnemiesToSpawn = 5;

    private List<Transform> spawnPoints = new List<Transform>();
    private List<GameObject> activeEnemies = new List<GameObject>();

    [SerializeField] private bool continousSpawner;

    void Start()
    {
        FindSpawnPoints();
        SpawnEnemies();
    }

    void Update()
    {
        if (continousSpawner && AllEnemiesDestroyed())
        {
            SpawnEnemies();
        }
    }

    void FindSpawnPoints()
    {
        spawnPoints.Clear();
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("EnemySpawnPoint");

        foreach (GameObject obj in spawnPointObjects)
            spawnPoints.Add(obj.transform);
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0)
            return;

        HashSet<int> usedSpawnPoints = new HashSet<int>();
        activeEnemies.Clear();

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

            GameObject spawnedEnemy = Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(spawnedEnemy);
        }
    }

    bool AllEnemiesDestroyed()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
        return activeEnemies.Count == 0;
    }

}
